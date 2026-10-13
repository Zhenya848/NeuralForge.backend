using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeuralForge.Application.Messages.Send;
using NeuralForge.Presentation.Extensions;
using NeuralForge.Presentation.Requests.Messages;

namespace NeuralForge.Presentation.Controllers;

public class MessageController : ApplicationController
{
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Send(
        [FromBody] SendMessageRequest request,
        [FromServices] SendMessageHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserIdRequired();
        
        var command = new SendMessageCommand(
            userId, request.ChatId, request.Prompt, request.Model, request.Base64File);
        
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse(StatusCodes.Status201Created);
    }
}