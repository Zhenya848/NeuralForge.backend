using ActsValidator.Presentation;
using CSharpFunctionalExtensions;
using Grpc.Core;
using NeuralForge.Application.Abstractions;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Presentation.Grpc.Services;

public class GreeterService : IGreeterService
{
    private readonly Greeter.GreeterClient _client;

    public GreeterService(Greeter.GreeterClient client)
    {
        _client = client;
    }

    public async Task<Result<string, ErrorList>> SendToAi(
        Guid userId, 
        Guid chatId, 
        string prompt, 
        string aiModel, 
        CancellationToken cancellationToken = default,
        string? base64File = null)
    {
        try
        {
            var request = new SendToAiRequest() 
                { UserId = userId.ToString(), ChatId = chatId.ToString(), Prompt = prompt, AiModel = aiModel, Base64File = base64File ?? "" };

            var result = await _client.SendToAiAsync(request, cancellationToken: cancellationToken);

            return result.Message;
        }
        catch (RpcException ex)
        {
            var splitMessage = ex.Status.Detail.Split(": ");
            
            if (splitMessage.Length > 1)
                return (ErrorList)Error.Failure(splitMessage[0], splitMessage[1]);
            
            return (ErrorList)Error.Failure("subtract.token.failure", ex.Status.Detail);
        }
    }
}