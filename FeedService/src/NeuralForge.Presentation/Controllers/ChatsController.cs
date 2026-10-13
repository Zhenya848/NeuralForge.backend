using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeuralForge.Application.ChatFolders.Delete;
using NeuralForge.Application.Chats.Create;
using NeuralForge.Application.Chats.Delete;
using NeuralForge.Application.Chats.Update;
using NeuralForge.Presentation.Extensions;
using NeuralForge.Presentation.Requests.Chats;
using NeuralForge.Presentation.Requests.Projects;

namespace NeuralForge.Presentation.Controllers;

public class ChatsController : ApplicationController
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateChatRequest request,
        [FromServices] CreateChatHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse("952f1aba-fa4c-48ae-9cf8-7eee59fc997f");
        
        var command = new CreateChatCommand(
            userId, request.Name, request.ProjectId, request.FolderId);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse(StatusCodes.Status201Created);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        [FromServices] DeleteChatHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse("952f1aba-fa4c-48ae-9cf8-7eee59fc997f");
        
        var command = new DeleteChatCommand(userId, id);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse();
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateChatRequest request,
        [FromServices] UpdateChatHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse("952f1aba-fa4c-48ae-9cf8-7eee59fc997f");
        
        var command = new UpdateChatCommand(
            userId, id, request.Name, request.ProjectId, request.FolderId);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse(StatusCodes.Status201Created);
    }
}