using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Chats;
using NeuralForge.Domain.Chats.ValueObjects.Ids;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;
using NeuralForge.Infrastructure.DbContexts;

namespace NeuralForge.Infrastructure.Repositories;

public class ChatsRepository : IChatsRepository
{
    private readonly AppDbContext _dbContext;

    public ChatsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Guid Add(Chat chat)
    {
        _dbContext.Chats.Add(chat);
        
        return chat.Id;
    }

    public Guid Delete(Chat chat)
    {
        _dbContext.Chats.Remove(chat);
        
        return chat.Id;
    }

    public async Task<Result<Chat, ErrorList>> GetById(
        Guid userId,
        ChatId chatId, 
        CancellationToken cancellationToken = default)
    {
        var chat = await _dbContext.Chats
            .FirstOrDefaultAsync(c =>
                c.Id == chatId &&
                c.UserId == userId,
                cancellationToken);

        if (chat is null)
            return (ErrorList)Errors.General.NotFound(chatId);
        
        return chat;
    }

    public async Task<Result<FolderId, ErrorList>> ResolveFolderIdForChat(
        Guid userId, ProjectId? projectId, FolderId? folderId, CancellationToken cancellationToken)
    {
        if (folderId is not null)
        {
            var exists = await _dbContext.ChatFolders.AnyAsync(
                f => f.UserId == userId && f.Id == folderId,
                cancellationToken);

            if (!exists)
                return (ErrorList)Errors.General.NotFound(folderId);

            return folderId;
        }
        
        if (projectId is null)
        {
            projectId = await _dbContext.Projects
                .Where(p => p.UserId == userId && p.IsSystem)
                .Select(p => p.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (projectId is null)
                return (ErrorList)Errors.General.NotFound();
        }
        
        var systemFolderId = await _dbContext.ChatFolders
            .Where(f =>
                f.UserId == userId &&
                f.ProjectId == projectId &&
                f.IsSystem)
            .Select(f => f.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (systemFolderId is null)
            return (ErrorList)Errors.General.NotFound();

        return systemFolderId;
    }
}