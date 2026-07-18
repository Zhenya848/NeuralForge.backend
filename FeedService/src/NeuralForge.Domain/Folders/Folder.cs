using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Domain.Folders;

public abstract class Folder : Shared.Entity<FolderId>
{
    public Guid UserId { get; }
    public FolderId? ParentId { get; }
    public string Name { get; protected set; }
    public int EmojiId { get; protected set; } = 0;

    [JsonConstructor]
    protected Folder(FolderId id) : base(id)
    {
        
    }
    
    protected Folder(FolderId id, Guid userId, FolderId? parentId, string name, int emojiId) : base(id)
    {
        UserId = userId;
        ParentId = parentId;
        Name = name;
        EmojiId = emojiId;
    }
    
    public UnitResult<ErrorList> Update(string name, int emojiId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (ErrorList)Errors.General.ValueIsRequired("name");
        
        if (emojiId < 0)
            return (ErrorList)Errors.General.ValueIsInvalid("emojiId");
        
        Name = name;
        EmojiId = emojiId;
        
        return UnitResult.Success<ErrorList>();
    }
}