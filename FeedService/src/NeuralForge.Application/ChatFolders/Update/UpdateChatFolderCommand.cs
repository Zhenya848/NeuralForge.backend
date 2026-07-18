namespace NeuralForge.Application.ChatFolders.Update;

public record UpdateChatFolderCommand(
    Guid UserId, 
    Guid FragmentFolderId, 
    string Name, 
    int EmojiId, 
    Guid? ParentId = null, 
    Guid? ProjectId = null);