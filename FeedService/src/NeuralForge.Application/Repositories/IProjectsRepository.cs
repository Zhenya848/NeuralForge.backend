using CSharpFunctionalExtensions;
using NeuralForge.Domain.Projects;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.Repositories;

public interface IProjectsRepository : IRepository
{
    public Guid Create(Project project);
    public Guid Delete(Project project);
    public Task<Result<Project, ErrorList>> GetById(
        Guid userId, 
        ProjectId id, 
        CancellationToken cancellationToken = default);
    public Task<Project> GetDefaultProject(Guid userId, CancellationToken cancellationToken = default);
}