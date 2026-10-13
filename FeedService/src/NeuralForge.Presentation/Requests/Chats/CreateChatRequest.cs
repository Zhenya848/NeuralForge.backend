namespace NeuralForge.Presentation.Requests.Chats;

public record CreateChatRequest(string Name, Guid? ProjectId = null, Guid? FolderId = null);