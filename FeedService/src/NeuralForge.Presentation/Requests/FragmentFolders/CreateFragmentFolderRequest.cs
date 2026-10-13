namespace NeuralForge.Presentation.Requests.FragmentFolders;

public record CreateFragmentFolderRequest(
    string Name, 
    int EmojiId, 
    Guid? ParentId = null, 
    Guid? ProjectId = null);