using CSharpFunctionalExtensions;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.ChatFolders.Update;

public class UpdateChatFolderHandler : ICommandHandler<UpdateChatFolderCommand, Result<Guid, ErrorList>>
{
    private readonly IChatFoldersRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateChatFolderHandler(IChatFoldersRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateChatFolderCommand command, 
        CancellationToken cancellationToken = default)
    {
        var folderResult = await _repository
            .GetById(command.UserId, FolderId.Create(command.FragmentFolderId), cancellationToken);

        if (folderResult.IsFailure)
            return folderResult.Error;
        
        /*var parentId = command.ParentId is not null ? FolderId.Create(command.ParentId.Value) : null;
        FragmentFolder? parent = null;

        if (parentId is not null)
        {
            try
            {
                using var connection = _sqlConnectionFactory.Create();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return (ErrorList)Errors.General.Failure();
            }
        }*/
        
        var updateResult = folderResult.Value.Update(command.Name, command.EmojiId);
        
        if (updateResult.IsFailure)
            return updateResult.Error;
        
        await _unitOfWork.SaveChanges(cancellationToken);
        
        return folderResult.Value.Id.Value;
    }
}