namespace NeuralForge.Application.Chats.Update;

public record UpdateChatCommand(Guid UserId, Guid ChatId, string Name, Guid? ProjectId = null, Guid? FolderId = null);