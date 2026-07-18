using CSharpFunctionalExtensions;
using NeuralForge.Domain.Chats;
using NeuralForge.Domain.Chats.ValueObjects.Ids;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.Repositories;

public interface IChatsRepository : IRepository
{
    public Guid Delete(Chat chat);
    public Task<Result<Chat, ErrorList>> GetById(
        Guid userId,
        ChatId chatId, 
        CancellationToken cancellationToken = default);
}