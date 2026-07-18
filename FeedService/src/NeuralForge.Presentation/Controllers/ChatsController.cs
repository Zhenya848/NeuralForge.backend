using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeuralForge.Application.Chats.Create;
using NeuralForge.Presentation.Extensions;
using NeuralForge.Presentation.Requests.Chats;
using NeuralForge.Presentation.Requests.Projects;

namespace NeuralForge.Presentation.Controllers;

public class ChatsController : ApplicationController
{
    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateChatRequest request,
        [FromServices] CreateChatHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserIdRequired();
        
        var command = new CreateChatCommand(
            userId, request.Name, request.FolderId);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse(StatusCodes.Status201Created);
    }
}