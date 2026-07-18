namespace NeuralForge.Application.Projects.Commands.Update;

public record UpdateProjectCommand(Guid UserId, Guid ProjectId, string Name, string Description, int EmojiId, int ColorId);