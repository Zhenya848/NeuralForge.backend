namespace NeuralForge.Presentation.Requests.Chats;

public record UpdateChatRequest(Guid ChatId, string Name, Guid? FolderId = null);