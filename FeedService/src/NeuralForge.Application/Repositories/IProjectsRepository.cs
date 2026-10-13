using CSharpFunctionalExtensions;
using NeuralForge.Domain.Projects;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.Repositories;

public interface IProjectsRepository : IRepository
{
    public Guid Create(Project project);
    public Guid Delete(Project project);
    public Task<bool> IsProjectExist(Guid userId, ProjectId projectId, CancellationToken cancellationToken = default);
    public Task<Result<Project, ErrorList>> GetById(
        Guid userId, 
        ProjectId id, 
        CancellationToken cancellationToken = default);
    public Task<List<Project>> GetByIds(
        Guid userId, 
        IEnumerable<ProjectId> projectIds, 
        CancellationToken cancellationToken = default);
    public Task<ProjectId> GetDefaultProjectId(Guid userId, CancellationToken cancellationToken = default);
}