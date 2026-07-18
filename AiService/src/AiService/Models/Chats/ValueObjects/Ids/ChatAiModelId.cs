namespace AiService.Models.Chats.ValueObjects.Ids;

public record ChatAiModelId
{
    public Guid Value { get; }
    
    public ChatAiModelId(Guid value) => Value = value;
    
    public static ChatAiModelId AddNewId() => new (Guid.NewGuid());
    
    public static ChatAiModelId AddEmptyId() => new (Guid.Empty);
    
    public static ChatAiModelId Create(Guid id) => new (id);

    public static implicit operator Guid(ChatAiModelId id)
    {
        ArgumentNullException.ThrowIfNull(id);
        
        return id.Value;
    }
}