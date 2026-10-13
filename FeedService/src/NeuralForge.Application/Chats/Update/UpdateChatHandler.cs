using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Chats.ValueObjects.Ids;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Projects;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;
using NeuralForge.Domain.Shared.ValueObjects.Dtos;

namespace NeuralForge.Application.Chats.Update;

public class UpdateChatHandler : ICommandHandler<UpdateChatCommand, Result<Guid, ErrorList>>
{
    private readonly IChatsRepository _repository;
    private readonly IChatFoldersRepository _chatFoldersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateChatHandler> _logger;

    public UpdateChatHandler(
        IChatsRepository repository,
        IChatFoldersRepository chatFoldersRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateChatHandler> logger)
    {
        _repository = repository;
        _chatFoldersRepository = chatFoldersRepository;
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
        var projectId = command.ProjectId is not null ? ProjectId.Create(command.ProjectId.Value) : null;

        if (projectId is not null && command.FolderId is null)
        {
            var systemFolder = await _chatFoldersRepository
                .GetDefaultFolderId(command.UserId, projectId, cancellationToken);
                
            chatResult.Value.Update(chatResult.Value.Name, systemFolder);
        }

        if (folderId is not null)
        {
            var folderResult = await _chatFoldersRepository
                .GetById(command.UserId, folderId, cancellationToken);
            
            if (folderResult.IsFailure)
                return folderResult.Error;
        }

        var updateResult = chatResult.Value
            .Update(command.Name, folderId);
        
        if (updateResult.IsFailure)
            return updateResult.Error;
        
        await _unitOfWork.SaveChanges(cancellationToken);
        
        return chatResult.Value.Id.Value;
    }
}