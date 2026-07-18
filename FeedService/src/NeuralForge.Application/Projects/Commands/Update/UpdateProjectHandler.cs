using CSharpFunctionalExtensions;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.Projects.Commands.Update;

public class UpdateProjectHandler : ICommandHandler<UpdateProjectCommand, Result<Guid, ErrorList>>
{
    private readonly IProjectsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectHandler(IProjectsRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateProjectCommand command, 
        CancellationToken cancellationToken = default)
    {
        var projectResult = await _repository
            .GetById(command.UserId, ProjectId.Create(command.ProjectId), cancellationToken);

        if (projectResult.IsFailure)
            return projectResult.Error;

        var updateResult = projectResult.Value
            .Update(command.Name, command.Description, command.EmojiId, command.ColorId);
        
        if (updateResult.IsFailure)
            return updateResult.Error;

        await _unitOfWork.SaveChanges(cancellationToken);
        
        return projectResult.Value.Id.Value;
    }
}