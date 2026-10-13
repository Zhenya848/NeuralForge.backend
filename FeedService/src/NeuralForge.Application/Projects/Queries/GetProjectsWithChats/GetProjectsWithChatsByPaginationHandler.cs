using System.Text.Json;
using Dapper;
using Microsoft.Extensions.Logging;
using NeuralForge.Application.Abstractions;
using NeuralForge.Domain.Shared;
using NeuralForge.Domain.Shared.ValueObjects.Dtos;

namespace NeuralForge.Application.Projects.Queries.GetProjectsWithChats;

public class GetProjectsWithChatsByPaginationHandler : IQueryHandler<GetProjectsWithChatsByPaginationQuery, PagedList<ProjectDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly ILogger<GetProjectsWithChatsByPaginationHandler> _logger;

    public GetProjectsWithChatsByPaginationHandler(
        ISqlConnectionFactory sqlConnectionFactory, 
        ILogger<GetProjectsWithChatsByPaginationHandler> logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _logger = logger;
    }
    
    public async Task<PagedList<ProjectDto>> Handle(
        GetProjectsWithChatsByPaginationQuery query, 
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
                       p.color_id,
                       p.is_system
                   FROM projects p
                   WHERE p.user_id = @UserId
                   ORDER BY p.name
                   LIMIT @ProjectsLimit
                   OFFSET @Offset
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
                       FROM chat_folders f
                       WHERE f.user_id = @UserId
                         AND f.project_id IN (SELECT id FROM selected_projects)
                   ) t
                   WHERE rn <= @FoldersPerProjectLimit
               ),
               
               chat_models AS (
                   SELECT
                       chat_id,
                       array_agg(DISTINCT ai_model_name) AS models
                   FROM messages
                   WHERE ai_model_name IS NOT NULL
                   GROUP BY chat_id
               ),
               
               selected_chats AS (
                   SELECT *
                   FROM (
                       SELECT
                           c.id,
                           c.user_id,
                           c.folder_id,
                           c.name,
                           c.connections,
                           cm.models,
                           ROW_NUMBER() OVER (
                               PARTITION BY c.folder_id
                               ORDER BY c.name DESC
                           ) AS rn
                       FROM chats c
                       LEFT JOIN chat_models cm
                           ON cm.chat_id = c.id
                       WHERE c.user_id = @UserId
                         AND c.folder_id IN (SELECT id FROM selected_folders)
                   ) t
                   WHERE t.rn <= @ChatsPerFolderLimit
                   LIMIT @ChatsTotalLimit
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
                                   'IsSystem', p.is_system,
                               
                                   'ChatFolders',
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
                                                   'IsSystem', f.is_system,
                               
                                                   'Chats',
                                                   COALESCE(
                                                       (
                                                           SELECT jsonb_agg(
                                                               jsonb_build_object(
                                                                   'Id', c.id,
                                                                   'UserId', c.user_id,
                                                                   'FolderId', c.folder_id,
                                                                   'Name', c.name,
                                                                   'Connections', c.connections,
                                                                   'UsedAiModels', COALESCE(c.models, ARRAY[]::text[])
                                                               )
                                                               ORDER BY c.name DESC
                                                           )
                                                           FROM selected_chats c
                                                           WHERE c.folder_id = f.id
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
                Page = query.Page,
                ProjectsLimit = pageSize,
                Offset = (query.Page - 1) * pageSize,
                FoldersPerProjectLimit = Constants.LoadLimits.MaxFoldersPerProject,
                ChatsPerFolderLimit = Constants.LoadLimits.MaxChatsPerFolder,
                ChatsTotalLimit = Constants.LoadLimits.MaxChatsTotal
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