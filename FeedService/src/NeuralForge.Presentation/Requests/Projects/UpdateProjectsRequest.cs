using NeuralForge.Application.Projects.Models;

namespace NeuralForge.Presentation.Requests.Projects;

public record UpdateProjectsRequest(UpdateProjectDto[] Projects);