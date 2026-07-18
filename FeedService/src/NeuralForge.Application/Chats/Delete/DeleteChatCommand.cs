namespace NeuralForge.Application.Chats.Delete;

public record DeleteChatCommand(Guid UserId, Guid ChatId);