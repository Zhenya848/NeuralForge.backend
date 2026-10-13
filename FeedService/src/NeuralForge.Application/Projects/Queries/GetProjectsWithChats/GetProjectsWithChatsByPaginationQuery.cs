namespace NeuralForge.Application.Projects.Queries.GetProjectsWithChats;

public record GetProjectsWithChatsByPaginationQuery(
    Guid UserId,
    int Page,
    int PageSize);