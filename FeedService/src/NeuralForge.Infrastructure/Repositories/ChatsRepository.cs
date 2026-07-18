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
}