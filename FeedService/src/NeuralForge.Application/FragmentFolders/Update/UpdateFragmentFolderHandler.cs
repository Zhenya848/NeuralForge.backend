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

namespace NeuralForge.Application.FragmentFolders.Update;

public class UpdateFragmentFolderHandler : ICommandHandler<UpdateFragmentFolderCommand, Result<Guid, ErrorList>>
{
    private readonly IFragmentFoldersRepository _repository;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateFragmentFolderHandler> _logger;

    public UpdateFragmentFolderHandler(
        IFragmentFoldersRepository repository, 
        ISqlConnectionFactory sqlConnectionFactory,
        IUnitOfWork unitOfWork,
        ILogger<UpdateFragmentFolderHandler> logger)
    {
        _repository = repository;
        _sqlConnectionFactory = sqlConnectionFactory;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateFragmentFolderCommand command, 
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