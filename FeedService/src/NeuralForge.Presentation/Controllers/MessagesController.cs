using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeuralForge.Application.Messages.Send;
using NeuralForge.Presentation.Extensions;
using NeuralForge.Presentation.Requests;
using NeuralForge.Presentation.Requests.Messages;

namespace NeuralForge.Presentation.Controllers;

public class MessagesController : ApplicationController
{
    [Authorize]
    [HttpPost("send")]
    public async Task<IActionResult> Send(
        [FromBody] SendMessageRequest request,
        [FromServices] SendMessageHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserIdRequired();
        
        var sendMessageCommand = new SendMessageCommand(
            userId, request.ChatId, request.Prompt, request.Model);

        var result = await handler.Handle(sendMessageCommand, cancellationToken);
        
        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.Value.ToResponse();
    }
}