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

public class ChatFoldersRepository : IChatFoldersRepository
{
    private readonly AppDbContext _dbContext;

    public ChatFoldersRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Guid Create(ChatFolder chatFolder)
    {
        _dbContext.ChatFolders.Add(chatFolder);
        
        return chatFolder.Id;
    }

    public Guid Delete(ChatFolder chatFolder)
    {
        _dbContext.ChatFolders.Remove(chatFolder);
        
        return chatFolder.Id;
    }

    public async Task<Result<ChatFolder, ErrorList>> GetById(Guid userId, FolderId id, CancellationToken cancellationToken = default)
    {
        var chatFolder = await _dbContext.ChatFolders
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken);

        if (chatFolder is null)
            return (ErrorList)Errors.General.NotFound(id);
            
        return chatFolder;
    }

    public Task<bool> IsFolderExist(Guid userId, ProjectId projectId, FolderId id, CancellationToken cancellationToken = default)
    {
        return _dbContext.ChatFolders.AnyAsync
            (c => c.Id == id && c.UserId == userId && c.ProjectId == projectId, cancellationToken);
    }

    public async Task<FolderId> GetDefaultFolderId(Guid userId, ProjectId? projectId, CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.ChatFolders
            .FirstOrDefaultAsync(f =>
                f.IsSystem && f.UserId == userId && (projectId == null || f.ProjectId == projectId), 
                cancellationToken);

        if (result is not null) 
            return result.Id;
        
        var defaultProject = await _dbContext.Projects.FirstOrDefaultAsync(p => 
            p.UserId == userId && p.IsSystem, 
            cancellationToken);

        if (defaultProject is null)
        {
            defaultProject = Project.Create(userId, Constants.DefaultProjectName, "", 0, 0, true).Value;
            _dbContext.Projects.Add(defaultProject);
        }

        result = ChatFolder.Create(userId, Constants.DefaultFolderName, defaultProject.Id).Value;
        defaultProject.AddFolder(result);

        return result.Id;
    }

    public async Task<bool> HasFolderWithName(
        Guid userId, 
        string name, 
        CancellationToken cancellationToken = default, 
        ProjectId? projectId = null,
        FolderId? parentId = null)
    {
        var result = await _dbContext.ChatFolders.AnyAsync(f =>
            f.UserId == userId && 
            (parentId == null || f.ParentId == parentId) && 
            (projectId == null || f.ProjectId == projectId) && 
            f.Name == name, 
            cancellationToken);
        
        return result;
    }
}