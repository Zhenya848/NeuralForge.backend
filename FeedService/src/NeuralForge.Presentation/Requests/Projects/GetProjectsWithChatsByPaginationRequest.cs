namespace NeuralForge.Presentation.Requests.Projects;

public record GetProjectsWithChatsByPaginationRequest(
    int Page,
    int PageSize,
    string? ProjectNameFilter = null);