namespace NeuralForge.Domain.Shared.ValueObjects.Dtos;

public record FolderDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid? ParentId { get; init; }
    public string Name { get; init; }
    public int EmojiId { get; init; } = 0;
}