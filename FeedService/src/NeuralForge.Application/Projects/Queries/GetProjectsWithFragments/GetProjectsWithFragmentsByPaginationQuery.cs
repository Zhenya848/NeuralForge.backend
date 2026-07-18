namespace NeuralForge.Application.Projects.Queries.GetProjectsWithFragments;

public record GetProjectsWithFragmentsByPaginationQuery(
    Guid UserId,
    int Page,
    int PageSize,
    string? ProjectNameFilter = null,
    string? NameFilter = null);