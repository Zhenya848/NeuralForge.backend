using CSharpFunctionalExtensions;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Chats;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Projects;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.Chats.Create;

public class CreateChatHandler : ICommandHandler<CreateChatCommand, Result<Guid, ErrorList>>
{
    private readonly IChatsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateChatHandler(IChatsRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        CreateChatCommand command, 
        CancellationToken cancellationToken = default)
    {
        var projectId = command.ProjectId is not null ? ProjectId.Create(command.ProjectId.Value) : null;
        var folderId = command.FolderId is not null ? FolderId.Create(command.FolderId.Value) : null;
        
        var folderIdResult = await _repository.ResolveFolderIdForChat(
            command.UserId,
            projectId,
            folderId,
            cancellationToken);
        
        if (folderIdResult.IsFailure)
            return folderIdResult.Error;

        var chatResult = Chat.Create(command.UserId, command.Name, folderIdResult.Value);
        
        if (chatResult.IsFailure)
            return chatResult.Error;
        
        _repository.Add(chatResult.Value);
        await _unitOfWork.SaveChanges(cancellationToken);
        
        return chatResult.Value.Id.Value;
    }
}