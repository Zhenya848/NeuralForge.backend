using CSharpFunctionalExtensions;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.Projects.Commands.Update;

public class UpdateProjectsHandler : ICommandHandler<UpdateProjectsCommand, UnitResult<ErrorList>>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectsHandler(IProjectsRepository projectsRepository, IUnitOfWork unitOfWork)
    {
        _projectsRepository = projectsRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<UnitResult<ErrorList>> Handle(
        UpdateProjectsCommand command, 
        CancellationToken cancellationToken = default)
    {
        var updates = command.Projects
            .Select(dto => (Id: ProjectId.Create(dto.Id), Dto: dto))
            .ToList();

        var projects = (await _projectsRepository
            .GetByIds(command.UserId, updates.Select(i => i.Id), cancellationToken))
            .ToDictionary(p => p.Id, p => p);
        
        var errors = new List<Error>();

        foreach (var u in updates)
        {
            if (projects.TryGetValue(ProjectId.Create(u.Id), out var project) == false)
                continue;

            var result = project.Update(
                u.Dto.Name,
                u.Dto.Description,
                u.Dto.EmojiId,
                u.Dto.ColorId);

            if (result.IsFailure)
                errors.AddRange(result.Error);
        }

        if (errors.Count > 0)
            return (ErrorList)errors;
        
        await _unitOfWork.SaveChanges(cancellationToken);

        return Result.Success<ErrorList>();
    }
}