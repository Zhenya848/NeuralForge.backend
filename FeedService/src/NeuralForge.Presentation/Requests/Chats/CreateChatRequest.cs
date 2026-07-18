namespace NeuralForge.Presentation.Requests.Chats;

public record CreateChatRequest(string Name, Guid? FolderId = null);