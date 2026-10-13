using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;
using NeuralForge.Domain.Shared.ValueObjects.Dtos;

namespace NeuralForge.Application.ChatFolders.Update;

public class UpdateChatFolderHandler : ICommandHandler<UpdateChatFolderCommand, Result<Guid, ErrorList>>
{
    private readonly IChatFoldersRepository _chatFoldersRepository;
    private readonly IProjectsRepository _projectsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly ILogger<UpdateChatFolderHandler> _logger;

    public UpdateChatFolderHandler(
        IChatFoldersRepository chatFoldersRepository, 
        IProjectsRepository projectsRepository,
        IUnitOfWork unitOfWork, 
        ISqlConnectionFactory sqlConnectionFactory,
        ILogger<UpdateChatFolderHandler> logger)
    {
        _chatFoldersRepository = chatFoldersRepository;
        _projectsRepository = projectsRepository;
        _unitOfWork = unitOfWork;
        _sqlConnectionFactory = sqlConnectionFactory;
        _logger = logger;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateChatFolderCommand command, 
        CancellationToken cancellationToken = default)
    {
        var folderResult = await _chatFoldersRepository
            .GetById(command.UserId, FolderId.Create(command.FragmentFolderId), cancellationToken);

        if (folderResult.IsFailure)
            return folderResult.Error;

        var projectId = command.ProjectId is not null ? ProjectId.Create(command.ProjectId.Value) : null;

        if (projectId is not null)
        {
            var isFolderExist = await _chatFoldersRepository.HasFolderWithName(
                command.UserId, 
                folderResult.Value.Name, 
                cancellationToken, 
                projectId);
            
            if (isFolderExist)
                return (ErrorList)Error.Failure(
                    "move.folder.failure", 
                    "Folder with this name already exists in this project");
            
            var isProjectExist = await _projectsRepository
                .IsProjectExist(command.UserId, projectId, cancellationToken);

            if (isProjectExist == false)
                return (ErrorList)Errors.General.NotFound(projectId);
            
            folderResult.Value.ChangeProjectId(projectId);
        }

        if (command.ParentId is not null)
        {
            try
            {
                using var connection = _sqlConnectionFactory.Create();

                var sql = """
                          WITH RECURSIVE parents AS (
                              SELECT id, parent_id
                              FROM chat_folders
                              WHERE id = @TargetId
                          
                              UNION ALL
                          
                              SELECT f.id, f.parent_id
                              FROM chat_folders f
                              JOIN parents p ON p.parent_id = f.id
                          )
                          SELECT EXISTS (
                              SELECT 1
                              FROM parents
                              WHERE id = @SourceId
                          );
                          """;

                var result = await connection.QuerySingleAsync<bool>(sql, connection);
            
                if (result)
                    return (ErrorList)Error.Conflict(
                        "moving.folder.conflict", 
                        "Unable to move target folder to source");
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return (ErrorList)Error.Failure("server.internal.error", e.Message);
            }
        }
        
        var updateResult = folderResult.Value.Update(command.Name, command.EmojiId);
        
        if (updateResult.IsFailure)
            return updateResult.Error;
        
        await _unitOfWork.SaveChanges(cancellationToken);
        
        return folderResult.Value.Id.Value;
    }
}