using NeuralForge.Application.Projects.Models;

namespace NeuralForge.Application.Projects.Commands.Update;

public record UpdateProjectsCommand(Guid UserId, UpdateProjectDto[] Projects);