using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeuralForge.Application.ChatFolders.Create;
using NeuralForge.Application.ChatFolders.Delete;
using NeuralForge.Application.ChatFolders.Update;
using NeuralForge.Application.Chats.Create;
using NeuralForge.Presentation.Extensions;
using NeuralForge.Presentation.Requests.ChatFolders;
using NeuralForge.Presentation.Requests.Chats;

namespace NeuralForge.Presentation.Controllers;

public class ChatFoldersController : ApplicationController
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateChatFolderRequest request,
        [FromServices] CreateChatFolderHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse("952f1aba-fa4c-48ae-9cf8-7eee59fc997f");
        
        var command = new CreateChatFolderCommand(
            userId, request.Name, request.EmojiId, request.ProjectId, request.ParentId);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse(StatusCodes.Status201Created);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        [FromServices] DeleteChatFolderHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse("952f1aba-fa4c-48ae-9cf8-7eee59fc997f");
        
        var command = new DeleteChatFolderCommand(userId, id);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse();
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateChatFolderRequest request,
        [FromServices] UpdateChatFolderHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse("952f1aba-fa4c-48ae-9cf8-7eee59fc997f");
        
        var command = new UpdateChatFolderCommand(
            userId, id, request.Name, request.EmojiId, request.ParentId, request.ProjectId);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse(StatusCodes.Status201Created);
    }
}