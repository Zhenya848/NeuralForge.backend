using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeuralForge.Application.Chats.Create;
using NeuralForge.Application.Chats.Delete;
using NeuralForge.Application.Chats.Update;
using NeuralForge.Application.Fragments.Create;
using NeuralForge.Application.Fragments.Delete;
using NeuralForge.Application.Fragments.Update;
using NeuralForge.Presentation.Extensions;
using NeuralForge.Presentation.Requests.Chats;
using NeuralForge.Presentation.Requests.Fragments;

namespace NeuralForge.Presentation.Controllers;

public class FragmentsController : ApplicationController
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateFragmentRequest request,
        [FromServices] CreateFragmentHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserIdRequired();
        
        var command = new CreateFragmentCommand(
            userId, request.Name, request.Content, request.MessageId, request.ProjectId, request.FolderId);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse(StatusCodes.Status201Created);
    }
    
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        [FromServices] DeleteFragmentHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserIdRequired();
        
        var command = new DeleteFragmentCommand(userId, id);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse();
    }
    
    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateFragmentRequest request,
        [FromServices] UpdateFragmentHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserIdRequired();
        
        var command = new UpdateFragmentCommand(
            id, userId, request.Name, request.FolderId);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse();
    }
}