namespace NeuralForge.Domain.Shared.ValueObjects.Dtos;

public record ChatDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid FolderId { get; init; }
    public string Name { get; init; }
    
    public Guid[] Connections { get; init; } = [];
    public string[] UsedAiModels { get; init; } = [];
}