namespace NeuralForge.Domain.Projects.ValueObjects.Ids;

public record ProjectId
{
    public Guid Value { get; }
    
    public ProjectId(Guid value) => Value = value;
    
    public static ProjectId AddNewId() => new (Guid.NewGuid());
    
    public static ProjectId AddEmptyId() => new (Guid.Empty);
    
    public static ProjectId Create(Guid id) => new (id);

    public static implicit operator Guid(ProjectId id)
    {
        ArgumentNullException.ThrowIfNull(id);
        
        return id.Value;
    }
}