namespace NeuralForge.Domain.Shared.ValueObjects.Dtos;

public record FragmentDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid FolderId { get; init; }
    
    public string Name { get; init; }
    public string Content { get; init; }
    
    public Guid MessageId { get; init; }
    public string? aiModel { get; init; }
}