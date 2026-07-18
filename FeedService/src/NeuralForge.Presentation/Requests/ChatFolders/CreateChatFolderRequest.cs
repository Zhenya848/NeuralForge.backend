namespace NeuralForge.Presentation.Requests.ChatFolders;

public record CreateChatFolderRequest(
    string Name, 
    int EmojiId = 0, 
    Guid? ProjectId = null, 
    Guid? ParentId = null);