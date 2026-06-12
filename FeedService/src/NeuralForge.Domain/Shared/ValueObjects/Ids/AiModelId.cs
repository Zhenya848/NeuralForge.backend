namespace NeuralForge.Domain.Shared.ValueObjects.Ids;

public record AiModelId
{
    public Guid Value { get; }
    
    public AiModelId(Guid value) => Value = value;
    
    public static AiModelId AddNewId() => new (Guid.NewGuid());
    
    public static AiModelId AddEmptyId() => new (Guid.Empty);
    
    public static AiModelId Create(Guid id) => new (id);

    public static implicit operator Guid(AiModelId id)
    {
        ArgumentNullException.ThrowIfNull(id);
        
        return id.Value;
    }
}