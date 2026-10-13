using System.Text.Json;
using Dapper;
using Microsoft.Extensions.Logging;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Projects.Queries.GetProjectsWithChats;
using NeuralForge.Domain.Shared;
using NeuralForge.Domain.Shared.ValueObjects.Dtos;

namespace NeuralForge.Application.Projects.Queries.GetProjectsWithFragments;

public class GetProjectsWithFragmentsByPaginationHandler : IQueryHandler<GetProjectsWithFragmentsByPaginationQuery, PagedList<ProjectDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly ILogger<GetProjectsWithChatsByPaginationHandler> _logger;

    public GetProjectsWithFragmentsByPaginationHandler(
        ISqlConnectionFactory sqlConnectionFactory, 
        ILogger<GetProjectsWithChatsByPaginationHandler> logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _logger = logger;
    }
    
    public async Task<PagedList<ProjectDto>> Handle(
        GetProjectsWithFragmentsByPaginationQuery query, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = _sqlConnectionFactory.Create();
            
            var sql = """
               WITH selected_projects AS (
                   SELECT
                       p.id,
                       p.user_id,
                       p.name,
                       p.description,
                       p.emoji_id,
                       p.color_id
                   FROM projects p
                   WHERE p.user_id = @UserId
                   ORDER BY p.name
                   LIMIT @ProjectsLimit
                   OFFSET @Offset
               ),
               matched_fragments AS (
                   SELECT *
                   FROM fragments fr
                   WHERE fr.user_id = @UserId
                     AND (
                           @NameFilter IS NULL
                        OR to_tsvector(
                               'simple',
                               concat_ws(' ', fr.name, fr.content)
                           ) @@ websearch_to_tsquery('simple', @NameFilter)
                     )
               ),
               matched_folder_ids AS
               (
                   SELECT DISTINCT folder_id
                   FROM matched_fragments
               ),
               selected_folders AS (
                   SELECT *
                   FROM (
                       SELECT
                           f.*,
                           ROW_NUMBER() OVER (
                               PARTITION BY f.project_id
                               ORDER BY f.name
                           ) rn
                       FROM fragment_folders f
                       INNER JOIN selected_projects p ON p.id = f.project_id
                       LEFT JOIN matched_folder_ids mf
                           ON mf.folder_id = f.id
                       WHERE (
                           @NameFilter IS NULL
                           OR to_tsvector('simple', f.name)
                              @@ websearch_to_tsquery('simple', @NameFilter)
                           OR mf.folder_id IS NOT NULL
                         )
                   ) t
                   WHERE rn <= @FoldersPerProjectLimit
               ),
               selected_fragments AS (
                   SELECT *
                   FROM (
                       SELECT
                           mf.*,
                           ROW_NUMBER() OVER (
                               PARTITION BY mf.folder_id
                               ORDER BY mf.name DESC
                           ) rn
                       FROM matched_fragments mf
                       INNER JOIN selected_folders f ON f.id = mf.folder_id
                   ) t
                   WHERE rn <= @FragmentsPerFolderLimit
                   LIMIT @FragmentsTotalLimit
               )
               
               SELECT jsonb_build_object(
                   'Items', COALESCE(
                       (
                           SELECT jsonb_agg(
                               jsonb_build_object(
                                   'Id', p.id,
                                   'UserId', p.user_id,
                                   'Name', p.name,
                                   'Description', p.description,
                                   'EmojiId', p.emoji_id,
                                   'ColorId', p.color_id,
                               
                                   'FragmentFolders',
                                   COALESCE(
                                       (
                                           SELECT jsonb_agg(
                                               jsonb_build_object(
                                                   'Id', f.id,
                                                   'UserId', f.user_id,
                                                   'ParentId', f.parent_id,
                                                   'ProjectId', f.project_id,
                                                   'Name', f.name,
                                                   'EmojiId', f.emoji_id,
                               
                                                   'Fragments', 
                                                   COALESCE(
                                                       (
                                                           SELECT jsonb_agg(
                                                               jsonb_build_object(
                                                                   'Id', fr.id,
                                                                   'UserId', fr.user_id,
                                                                   'FolderId', fr.folder_id,
                                                                   'Name', fr.name,
                                                                   'Content', fr.content,
                                                                   'MessageId', fr.message_id
                                                               )
                                                               ORDER BY fr.name DESC
                                                           )
                                                           FROM selected_fragments fr
                                                           WHERE fr.folder_id = f.id
                                                       ),
                                                       '[]'::jsonb
                                                   )
                                               )
                                               ORDER BY f.name
                                           )
                                           FROM selected_folders f
                                           WHERE f.project_id = p.id
                                       ),
                                       '[]'::jsonb
                                   )
                               )
                               ORDER BY p.name)
                           FROM selected_projects p
                       ),
                       '[]'::jsonb
                   ),
                   'Page', @Page,
                   'PageSize', @ProjectsLimit,
                   'TotalCount',
                   (
                       SELECT COUNT(*)
                       FROM projects p
                       WHERE p.user_id = @UserId
                   )
               ) AS result;
               """;
            
            var pageSize = Math.Min(query.PageSize, Constants.LoadLimits.MaxProjects);

            var json = await connection.QuerySingleAsync<string>(sql, new
            {
                UserId = query.UserId,
                NameFilter = query.NameFilter,
                Page = query.Page,
                ProjectsLimit = pageSize,
                Offset = (query.Page - 1) * pageSize,
                FoldersPerProjectLimit = Constants.LoadLimits.MaxFoldersPerProject,
                FragmentsPerFolderLimit = Constants.LoadLimits.MaxChatsPerFolder,
                FragmentsTotalLimit = Constants.LoadLimits.MaxChatsTotal
            });
            
            var result = JsonSerializer.Deserialize<PagedList<ProjectDto>>(json) ?? new PagedList<ProjectDto>();

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return new PagedList<ProjectDto>();
        }
    }
}