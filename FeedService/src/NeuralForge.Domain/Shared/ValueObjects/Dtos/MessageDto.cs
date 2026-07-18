namespace NeuralForge.Domain.Shared.ValueObjects.Dtos;

public class MessageDto
{
    public Guid Id { get; init; }
    public Guid ChatId { get; init; }
    public int SerialNumber { get; init; }
    public string Value { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsBookmarked { get; init; }
}