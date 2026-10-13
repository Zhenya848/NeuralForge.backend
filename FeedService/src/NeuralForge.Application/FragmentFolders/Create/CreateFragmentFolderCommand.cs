namespace NeuralForge.Application.FragmentFolders.Create;

public record CreateFragmentFolderCommand(
    Guid UserId,
    string Name,
    int EmojiId,
    Guid? ProjectId = null,
    Guid? ParentId = null);