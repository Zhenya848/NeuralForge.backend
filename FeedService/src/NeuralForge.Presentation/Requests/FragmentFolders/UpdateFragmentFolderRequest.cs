namespace NeuralForge.Presentation.Requests.FragmentFolders;

public record UpdateFragmentFolderRequest(
    Guid FragmentFolderId, 
    string Name, 
    int EmojiId, 
    Guid? ParentId = null, 
    Guid? ProjectId = null);