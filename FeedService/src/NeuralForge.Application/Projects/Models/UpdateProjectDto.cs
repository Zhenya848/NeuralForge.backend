namespace NeuralForge.Application.Projects.Models;

public record UpdateProjectDto(Guid Id, string Name, string Description, int EmojiId, int ColorId);