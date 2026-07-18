using CSharpFunctionalExtensions;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.ChatFolders.Delete;

public class DeleteChatFolderHandler : ICommandHandler<DeleteChatFolderCommand, Result<Guid, ErrorList>>
{
    private readonly IChatFoldersRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteChatFolderHandler(IChatFoldersRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        DeleteChatFolderCommand command, 
        CancellationToken cancellationToken = default)
    {
        var folderResult = await _repository
            .GetById(command.UserId, FolderId.Create(command.FolderId), cancellationToken);

        if (folderResult.IsFailure)
            return folderResult.Error;

        var result = _repository.Delete(folderResult.Value);
        await _unitOfWork.SaveChanges(cancellationToken);
        
        return result;
    }
}