using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeuralForge.Application.FragmentFolders.Create;
using NeuralForge.Application.FragmentFolders.Delete;
using NeuralForge.Application.FragmentFolders.Update;
using NeuralForge.Presentation.Extensions;
using NeuralForge.Presentation.Requests.FragmentFolders;

namespace NeuralForge.Presentation.Controllers;

public class FragmentFoldersController : ApplicationController
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateFragmentFolderRequest request,
        [FromServices] CreateFragmentFolderHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserIdRequired();
        
        var command = new CreateFragmentFolderCommand(
            userId, request.Name, request.EmojiId, request.ProjectId, request.ParentId);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse(StatusCodes.Status201Created);
    }
    
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        [FromServices] DeleteFragmentFolderHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserIdRequired();
        
        var command = new DeleteFragmentFolderCommand(userId, id);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse();
    }
    
    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateFragmentFolderRequest request,
        [FromServices] UpdateFragmentFolderHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserIdRequired();
        
        var command = new UpdateFragmentFolderCommand(
            id, userId, request.Name, request.EmojiId);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse(StatusCodes.Status201Created);
    }
}