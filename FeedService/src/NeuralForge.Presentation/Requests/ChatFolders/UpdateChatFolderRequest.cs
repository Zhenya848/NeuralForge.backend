namespace NeuralForge.Presentation.Requests.ChatFolders;

public record UpdateChatFolderRequest(
    string Name, 
    int EmojiId, 
    Guid? ParentId = null, 
    Guid? ProjectId = null);