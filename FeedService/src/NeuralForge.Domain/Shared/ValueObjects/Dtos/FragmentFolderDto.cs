namespace NeuralForge.Domain.Shared.ValueObjects.Dtos;

public record FragmentFolderDto : FolderDto
{
    public Guid ProjectId { get; init; }
    private FragmentDto[] Fragments { get; init; } = [];
}