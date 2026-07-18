using CSharpFunctionalExtensions;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Chats;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.Chats.Create;

public class CreateChatHandler : ICommandHandler<CreateChatCommand, Result<Guid, ErrorList>>
{
    private readonly IChatFoldersRepository _repository;
    public readonly IUnitOfWork _unitOfWork;

    public CreateChatHandler(IChatFoldersRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        CreateChatCommand command, 
        CancellationToken cancellationToken = default)
    {
        ChatFolder folder;

        if (command.FolderId is not null)
        {
            var folderResult = await _repository.GetById(
                command.UserId, 
                FolderId.Create(command.FolderId.Value), 
                cancellationToken);

            if (folderResult.IsFailure)
                return folderResult.Error;
            
            folder = folderResult.Value;
        }
        else
            folder = await _repository.GetDefaultFolder(command.UserId, cancellationToken);

        var chatResult = Chat.Create(command.UserId, command.Name, folder);
        
        if (chatResult.IsFailure)
            return chatResult.Error;
        
        folder.AddChat(chatResult.Value);
        await _unitOfWork.SaveChanges(cancellationToken);
        
        return chatResult.Value.Id.Value;
    }
}