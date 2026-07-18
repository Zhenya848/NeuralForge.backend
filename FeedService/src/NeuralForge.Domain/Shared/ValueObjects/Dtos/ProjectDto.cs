namespace NeuralForge.Domain.Shared.ValueObjects.Dtos;

public record ProjectDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public int EmojiId { get; init; }
    public int ColorId { get; init; }
    public FragmentFolderDto[] FragmentFolders { get; init; } = [];
    public ChatFolderDto[] ChatFolders { get; init; } = [];
}