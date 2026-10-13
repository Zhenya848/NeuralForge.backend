using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeuralForge.Application.Projects.Commands.Create;
using NeuralForge.Application.Projects.Commands.Update;
using NeuralForge.Application.Projects.Queries;
using NeuralForge.Application.Projects.Queries.GetProjectsWithChats;
using NeuralForge.Application.Projects.Queries.GetProjectsWithFragments;
using NeuralForge.Presentation.Extensions;
using NeuralForge.Presentation.Requests.Projects;

namespace NeuralForge.Presentation.Controllers;

public class ProjectsController : ApplicationController
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateProjectRequest request,
        [FromServices] CreateProjectHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse("952f1aba-fa4c-48ae-9cf8-7eee59fc997f");
        
        var command = new CreateProjectCommand(
            userId, request.Name, request.Description, request.EmojiId, request.ColorId);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse(StatusCodes.Status201Created);
    }
    
    [HttpPut("{projectId:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid projectId,
        [FromBody] UpdateProjectRequest request,
        [FromServices] UpdateProjectHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse("952f1aba-fa4c-48ae-9cf8-7eee59fc997f");
        
        var command = new UpdateProjectCommand(
            userId, projectId, request.Name, request.Description, request.EmojiId, request.ColorId);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse();
    }
    
    [HttpPut]
    public async Task<IActionResult> Update(
        [FromBody] UpdateProjectsRequest request,
        [FromServices] UpdateProjectsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse("952f1aba-fa4c-48ae-9cf8-7eee59fc997f");
        
        var command = new UpdateProjectsCommand(userId, request.Projects);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok();
    }
    
    [HttpGet("chats")]
    public async Task<IActionResult> GetWithChatsByPagination(
        [FromQuery] GetProjectsWithChatsByPaginationRequest request,
        [FromServices] GetProjectsWithChatsByPaginationHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse("952f1aba-fa4c-48ae-9cf8-7eee59fc997f");
        
        var query = new GetProjectsWithChatsByPaginationQuery(
            userId, 
            request.Page, 
            request.PageSize);
        
        var result = await handler.Handle(query, cancellationToken);

        return result.ToResponse();
    }
    
    [Authorize]
    [HttpPost("fragments")]
    public async Task<IActionResult> GetWithFragmentsByPagination(
        [FromQuery] GetProjectsWithFragmentsByPaginationRequest request,
        [FromServices] GetProjectsWithFragmentsByPaginationHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserIdRequired();
        
        var query = new GetProjectsWithFragmentsByPaginationQuery(
            userId, 
            request.Page, 
            request.PageSize,
            request.NameFilter);
        
        var result = await handler.Handle(query, cancellationToken);

        return result.ToResponse();
    }
}