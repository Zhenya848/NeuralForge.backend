namespace AiService.Models.Chats.ValueObjects.Ids;

public record ChatMemoryId
{
    public Guid Value { get; }
    
    public ChatMemoryId(Guid value) => Value = value;
    
    public static ChatMemoryId AddNewId() => new (Guid.NewGuid());
    
    public static ChatMemoryId AddEmptyId() => new (Guid.Empty);
    
    public static ChatMemoryId Create(Guid id) => new (id);

    public static implicit operator Guid(ChatMemoryId id)
    {
        ArgumentNullException.ThrowIfNull(id);
        
        return id.Value;
    }
}