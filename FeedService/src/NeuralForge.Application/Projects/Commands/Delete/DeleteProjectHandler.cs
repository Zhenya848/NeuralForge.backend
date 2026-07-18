using CSharpFunctionalExtensions;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.Projects.Commands.Delete;

public class DeleteProjectHandler : ICommandHandler<DeleteProjectCommand, Result<Guid, ErrorList>>
{
    private readonly IProjectsRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProjectHandler(IProjectsRepository projectRepository, IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        DeleteProjectCommand command, 
        CancellationToken cancellationToken = default)
    {
        var projectResult = await _projectRepository
            .GetById(command.UserId, ProjectId.Create(command.ProjectId), cancellationToken);

        if (projectResult.IsFailure)
            return projectResult.Error;
        
        var result = _projectRepository.Delete(projectResult.Value);
        await _unitOfWork.SaveChanges(cancellationToken);
        
        return result;
    }
}