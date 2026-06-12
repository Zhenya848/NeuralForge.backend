namespace NeuralForge.Domain.Fragments.ValueObjects.Ids;

public record FragmentId
{
    public Guid Value { get; }
    
    public FragmentId(Guid value) => Value = value;
    
    public static FragmentId AddNewId() => new (Guid.NewGuid());
    
    public static FragmentId AddEmptyId() => new (Guid.Empty);
    
    public static FragmentId Create(Guid id) => new (id);

    public static implicit operator Guid(FragmentId id)
    {
        ArgumentNullException.ThrowIfNull(id);
        
        return id.Value;
    }
}