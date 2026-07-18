namespace NeuralForge.Application.Chats.Create;

public record CreateChatCommand(Guid UserId, string Name, Guid? FolderId = null);