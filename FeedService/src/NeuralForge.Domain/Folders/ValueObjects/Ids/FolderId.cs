namespace NeuralForge.Domain.Folders.ValueObjects.Ids;

public record FolderId
{
    public Guid Value { get; }
    
    public FolderId(Guid value) => Value = value;
    
    public static FolderId AddNewId() => new (Guid.NewGuid());
    
    public static FolderId AddEmptyId() => new (Guid.Empty);
    
    public static FolderId Create(Guid id) => new (id);

    public static implicit operator Guid(FolderId id)
    {
        ArgumentNullException.ThrowIfNull(id);
        
        return id.Value;
    }
}