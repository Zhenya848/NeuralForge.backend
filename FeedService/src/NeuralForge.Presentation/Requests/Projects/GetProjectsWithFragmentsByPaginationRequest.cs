namespace NeuralForge.Presentation.Requests.Projects;

public record GetProjectsWithFragmentsByPaginationRequest(
    int Page,
    int PageSize,
    string? NameFilter = null);