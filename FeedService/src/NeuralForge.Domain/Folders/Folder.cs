using CSharpFunctionalExtensions;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Domain.Folders;

public class Folder : Shared.Entity<FolderId>
{
    public FolderId? ParentId { get; }
    public string Name { get; }
    public int EmojiId { get; } = 0;
    
    protected Folder(FolderId id, FolderId? parentId, string name, int emojiId) : base(id)
    {
        ParentId = parentId;
        Name = name;
        EmojiId = emojiId;
    }
}