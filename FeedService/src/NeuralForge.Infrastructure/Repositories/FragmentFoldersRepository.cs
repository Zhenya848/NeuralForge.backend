using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Projects;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;
using NeuralForge.Infrastructure.DbContexts;

namespace NeuralForge.Infrastructure.Repositories;

public class FragmentFoldersRepository : IFragmentFoldersRepository
{
    private readonly AppDbContext _dbContext;

    public FragmentFoldersRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Guid Add(FragmentFolder folder)
    {
        _dbContext.FragmentFolders.Add(folder);
        
        return folder.Id;
    }

    public Guid Delete(FragmentFolder folder)
    {
        _dbContext.FragmentFolders.Remove(folder);
        
        return folder.Id;
    }

    public async Task<Result<FragmentFolder, ErrorList>> GetById(
        Guid userId, 
        FolderId id, 
        CancellationToken cancellationToken = default)
    {
        var chatFolder = await _dbContext.FragmentFolders
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken);

        if (chatFolder is null)
            return (ErrorList)Errors.General.NotFound(id);
            
        return chatFolder;
    }

    public Task<bool> IsFolderExist(Guid userId, ProjectId projectId, FolderId id, CancellationToken cancellationToken = default)
    {
        return _dbContext.FragmentFolders.AnyAsync
            (c => c.Id == id && c.UserId == userId && c.ProjectId == projectId, cancellationToken);
    }

    public async Task<FragmentFolder> GetDefaultFolder(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.FragmentFolders
            .FirstOrDefaultAsync(f =>
                    f.Name == Constants.DefaultFolderName &&
                    f.UserId  == userId, 
                cancellationToken);

        if (result is not null) 
            return result;
        
        var defaultProject = await _dbContext.Projects.FirstOrDefaultAsync(p => 
                p.UserId == userId && 
                p.Name == Constants.DefaultProjectName, 
            cancellationToken);

        if (defaultProject is null)
        {
            defaultProject = Project.Create(userId, Constants.DefaultProjectName, "", 0, 0, true).Value;
            _dbContext.Projects.Add(defaultProject);
        }

        result = FragmentFolder.Create(userId, Constants.DefaultFolderName, defaultProject.Id).Value;
        defaultProject.AddFolder(result);

        return result;
    }
}