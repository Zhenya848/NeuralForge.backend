namespace NeuralForge.Application.ChatFolders.Create;

public record CreateChatFolderCommand(
    Guid UserId, 
    string Name, 
    int EmojiId = 0, 
    Guid? ProjectId = null, 
    Guid? ParentId = null);