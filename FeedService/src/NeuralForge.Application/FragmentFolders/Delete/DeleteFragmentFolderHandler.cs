using CSharpFunctionalExtensions;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.FragmentFolders.Delete;

public class DeleteFragmentFolderHandler : ICommandHandler<DeleteFragmentFolderCommand, Result<Guid, ErrorList>>
{
    private readonly IFragmentFoldersRepository _repository;

    public DeleteFragmentFolderHandler(IFragmentFoldersRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        DeleteFragmentFolderCommand command, 
        CancellationToken cancellationToken = default)
    {
        var folderResult = await _repository
            .GetById(command.UserId, FolderId.Create(command.FragmentFolderId), cancellationToken);

        if (folderResult.IsFailure)
            return folderResult.Error;
        
        var result = _repository.Delete(folderResult.Value);
        
        return result;
    }
}