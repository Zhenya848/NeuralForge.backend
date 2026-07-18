using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Chats.ValueObjects.Ids;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;
using NeuralForge.Domain.Shared.ValueObjects.Dtos;

namespace NeuralForge.Application.Chats.Update;

public class UpdateChatHandler : ICommandHandler<UpdateChatCommand, Result<Guid, ErrorList>>
{
    private readonly IChatsRepository _repository;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateChatHandler> _logger;

    public UpdateChatHandler(
        IChatsRepository repository,
        ISqlConnectionFactory sqlConnectionFactory,
        IUnitOfWork unitOfWork,
        ILogger<UpdateChatHandler> logger)
    {
        _repository = repository;
        _sqlConnectionFactory = sqlConnectionFactory;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateChatCommand command, 
        CancellationToken cancellationToken = default)
    {
        var chatResult = await _repository
            .GetById(command.UserId, ChatId.Create(command.ChatId), cancellationToken);
        
        if (chatResult.IsFailure)
            return chatResult.Error;
        
        var folderId = command.FolderId is not null ? FolderId.Create(command.FolderId.Value) : null;
        ChatFolder? folder = null;

        if (folderId is not null)
        {
            try
            {
                using var connection = _sqlConnectionFactory.Create();

                var sqlMessageResult = """SELECT * FROM ChatFolders f WHERE f.id = @id LIMIT 1""";
                var messageResult = await connection
                    .QuerySingleAsync<ChatFolderDto>(sqlMessageResult, new { id = command.FolderId });

                folder = ChatFolder.Create(
                    messageResult.UserId,
                    messageResult.Name,
                    ProjectId.Create(messageResult.ProjectId)).Value;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return (ErrorList)Errors.General.Failure();
            }
        }

        var updateResult = chatResult.Value
            .Update(command.Name, folder);
        
        if (updateResult.IsFailure)
            return updateResult.Error;
        
        await _unitOfWork.SaveChanges(cancellationToken);
        
        return chatResult.Value.Id.Value;
    }
}