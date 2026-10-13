namespace NeuralForge.Application.FragmentFolders.Update;

public record UpdateFragmentFolderCommand(
    Guid UserId, 
    Guid FragmentFolderId, 
    string Name, 
    int EmojiId);