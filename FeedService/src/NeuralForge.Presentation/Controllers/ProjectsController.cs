using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeuralForge.Application.Projects.Commands.Create;
using NeuralForge.Application.Projects.Queries;
using NeuralForge.Application.Projects.Queries.GetProjectsWithChats;
using NeuralForge.Application.Projects.Queries.GetProjectsWithFragments;
using NeuralForge.Presentation.Extensions;
using NeuralForge.Presentation.Requests.Projects;

namespace NeuralForge.Presentation.Controllers;

public class ProjectsController : ApplicationController
{
    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateProjectRequest request,
        [FromServices] CreateProjectHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserIdRequired();
        
        var command = new CreateProjectCommand(
            userId, request.Name, request.Description, request.EmojiId, request.ColorId);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse(StatusCodes.Status201Created);
    }
    
    [Authorize]
    [HttpPost("chats/get")]
    public async Task<IActionResult> GetWithChatsByPagination(
        [FromBody] GetProjectsWithChatsByPaginationRequest request,
        [FromServices] GetProjectsWithChatsByPaginationHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserIdRequired();
        
        var query = new GetProjectsWithChatsByPaginationQuery(
            userId, 
            request.Page, 
            request.PageSize, 
            request.ProjectNameFilter);
        
        var result = await handler.Handle(query, cancellationToken);

        return result.ToResponse();
    }
    
    [Authorize]
    [HttpPost("fragments/get")]
    public async Task<IActionResult> GetWithFragmentsByPagination(
        [FromBody] GetProjectsWithFragmentsByPaginationRequest request,
        [FromServices] GetProjectsWithFragmentsByPaginationHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserIdRequired();
        
        var query = new GetProjectsWithFragmentsByPaginationQuery(
            userId, 
            request.Page, 
            request.PageSize, 
            request.ProjectNameFilter, 
            request.NameFilter);
        
        var result = await handler.Handle(query, cancellationToken);

        return result.ToResponse();
    }
}