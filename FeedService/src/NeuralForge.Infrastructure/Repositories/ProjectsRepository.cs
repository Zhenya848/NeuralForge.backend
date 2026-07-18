using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Projects;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;
using NeuralForge.Infrastructure.DbContexts;

namespace NeuralForge.Infrastructure.Repositories;

public class ProjectsRepository : IProjectsRepository
{
    private readonly AppDbContext _dbContext;

    public ProjectsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Guid Create(Project project)
    {
        _dbContext.Projects.Add(project);
        
        return project.Id;
    }

    public Guid Delete(Project project)
    {
        _dbContext.Projects.Remove(project);
        
        return project.Id;
    }

    public async Task<Result<Project, ErrorList>> GetById(
        Guid userId, 
        ProjectId id, 
        CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.Projects
            .Include(f => f.ChatFolders)
            .FirstOrDefaultAsync(p => p.UserId == userId && p.Id == id, cancellationToken);

        if (result is null)
            return (ErrorList)Errors.General.NotFound(id);

        return result;
    }

    public async Task<Project> GetDefaultProject(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.Projects
            .Include(f => f.ChatFolders)
            .FirstOrDefaultAsync(p => p.UserId == userId && p.Name == Constants.DefaultProjectName, cancellationToken);

        if (result is not null) 
            return result;
        
        result = Project.Create(userId, Constants.DefaultProjectName, "", 0, 0).Value;
        _dbContext.Projects.Add(result);

        return result;
    }
}