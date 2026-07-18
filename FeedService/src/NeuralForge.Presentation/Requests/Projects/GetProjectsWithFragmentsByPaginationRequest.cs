namespace NeuralForge.Presentation.Requests.Projects;

public record GetProjectsWithFragmentsByPaginationRequest(
    int Page,
    int PageSize,
    string? ProjectNameFilter = null,
    string? NameFilter = null);