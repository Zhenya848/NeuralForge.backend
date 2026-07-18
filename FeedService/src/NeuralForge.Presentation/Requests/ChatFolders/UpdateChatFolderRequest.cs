namespace NeuralForge.Presentation.Requests.ChatFolders;

public record UpdateChatFolderRequest(
    Guid FragmentFolderId, 
    string Name, 
    int EmojiId, 
    Guid? ParentId = null, 
    Guid? ProjectId = null);