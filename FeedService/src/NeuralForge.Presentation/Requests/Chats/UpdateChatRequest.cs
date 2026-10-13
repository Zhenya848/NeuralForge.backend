namespace NeuralForge.Presentation.Requests.Chats;

public record UpdateChatRequest(string Name, Guid? ProjectId = null, Guid? FolderId = null);