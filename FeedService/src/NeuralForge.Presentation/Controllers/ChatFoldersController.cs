using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeuralForge.Application.ChatFolders.Create;
using NeuralForge.Application.Chats.Create;
using NeuralForge.Presentation.Extensions;
using NeuralForge.Presentation.Requests.ChatFolders;
using NeuralForge.Presentation.Requests.Chats;

namespace NeuralForge.Presentation.Controllers;

public class ChatFoldersController : ApplicationController
{
    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateChatFolderRequest request,
        [FromServices] CreateChatFolderHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserIdRequired();
        
        var command = new CreateChatFolderCommand(
            userId, request.Name, request.EmojiId, request.ProjectId, request.ParentId);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse(StatusCodes.Status201Created);
    }
}