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
            .Include(c => c.Chats)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken);

        if (chatFolder is null)
            return (ErrorList)Errors.General.NotFound(id);
            
        return chatFolder;
    }

    public async Task<ChatFolder> GetDefaultFolder(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.ChatFolders
            .Include(c => c.Chats)
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
            defaultProject = Project.Create(userId, Constants.DefaultProjectName, "", 0, 0).Value;
            _dbContext.Projects.Add(defaultProject);
        }

        result = ChatFolder.Create(userId, Constants.DefaultFolderName, defaultProject.Id).Value;
        defaultProject.AddFolder(result);

        return result;
    }

    public async Task<bool> HasFolderWithName(
        Guid userId, 
        string name, 
        CancellationToken cancellationToken = default, 
        FolderId? parentId = null)
    {
        var result = await _dbContext.ChatFolders.AnyAsync(f =>
            f.UserId == userId && f.ParentId == parentId && f.Name == name, cancellationToken);
        
        return result;
    }
}