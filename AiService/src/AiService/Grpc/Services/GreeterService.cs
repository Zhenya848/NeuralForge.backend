using AiService.Features;
using AiService.Features.Send;
using CSharpFunctionalExtensions;
using Grpc.Core;
using UserService.Presentation;

namespace AiService.Grpc.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly SendHandler _handler;

    public GreeterService(SendHandler handler)
    {
        _handler = handler;
    }
    
    public override async Task<SendToAiResponse> SendToAi(SendToAiRequest request, ServerCallContext context)
    {
        if (Guid.TryParse(request.UserId, out var userId))
            throw new RpcException(new Status(
                StatusCode.InvalidArgument, 
                "Invalid user id"
            ));
        
        if (Guid.TryParse(request.ChatId, out var chatId))
            throw new RpcException(new Status(
                StatusCode.InvalidArgument, 
                "Invalid chat id"
            ));
        
        var command = new SendCommand(userId, chatId, request.Prompt, request.AiModel, request.Base64File);
        var result = await _handler.Handle(command, context.CancellationToken);
        
        if (result.IsFailure)
            throw new RpcException(new Status(
                StatusCode.FailedPrecondition, 
                $"{result.Error.Code}: {result.Error.Message}"
            ));
        
        return new SendToAiResponse() { Message = result.Value };
    }
}