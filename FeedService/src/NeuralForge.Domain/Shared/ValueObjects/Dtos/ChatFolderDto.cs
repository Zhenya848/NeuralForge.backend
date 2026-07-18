namespace NeuralForge.Domain.Shared.ValueObjects.Dtos;

public record ChatFolderDto : FolderDto
{
    public Guid ProjectId { get; init; }
    public ChatDto[] Chats { get; init; } = [];
}