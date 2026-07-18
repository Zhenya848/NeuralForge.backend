namespace NeuralForge.Application.Projects.Commands.Delete;

public record DeleteProjectCommand(Guid UserId, Guid ProjectId);