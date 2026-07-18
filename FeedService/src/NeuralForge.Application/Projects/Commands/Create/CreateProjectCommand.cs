namespace NeuralForge.Application.Projects.Commands.Create;

public record CreateProjectCommand(Guid UserId, string Name, string Description, int EmojiId, int ColorId);