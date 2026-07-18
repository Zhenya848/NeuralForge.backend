using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Fragments.ValueObjects.Ids;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;
using NeuralForge.Domain.Shared.ValueObjects.Dtos;

namespace NeuralForge.Application.Fragments.Update;

public class UpdateFragmentHandler : ICommandHandler<UpdateFragmentCommand, Result<Guid, ErrorList>>
{
    private readonly IFragmentsRepository _repository;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateFragmentHandler> _logger;

    public UpdateFragmentHandler(
        IFragmentsRepository repository, 
        ISqlConnectionFactory sqlConnectionFactory, 
        IUnitOfWork unitOfWork,
        ILogger<UpdateFragmentHandler> logger)
    {
        _repository = repository;
        _sqlConnectionFactory = sqlConnectionFactory;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateFragmentCommand command, 
        CancellationToken cancellationToken = default)
    {
        var fragmentResult = await _repository
            .GetById(command.UserId, FragmentId.Create(command.FragmentId), cancellationToken);

        if (fragmentResult.IsFailure)
            return fragmentResult.Error;
        
        var folderId = command.FolderId is not null ? FolderId.Create(command.FolderId.Value) : null;
        FragmentFolder? folder = null;

        if (folderId is not null)
        {
            try
            {
                using var connection = _sqlConnectionFactory.Create();

                var sqlMessageResult = """SELECT * FROM FragmetnFolders f WHERE f.id = @id LIMIT 1""";
                var messageResult = await connection
                    .QuerySingleAsync<FragmentFolderDto>(sqlMessageResult, new { id = command.FolderId });

                folder = FragmentFolder.Create(
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

        var updateResult = fragmentResult.Value
            .Update(command.Name, folder);
        
        if (updateResult.IsFailure)
            return updateResult.Error;
        
        await _unitOfWork.SaveChanges(cancellationToken);
        
        return fragmentResult.Value.Id.Value;
    }
}