using AiService.Abstractions;
using AiService.Db;
using AiService.Extensions;
using AiService.Models.Shared;
using AiService.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AiService.Features;

public class SendHandler
{
    private record SendRequest(Guid ChatId, string Prompt, string Base64File);
    
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/AI/send", Handler);
        }
        
        private static async Task<IResult> Handler(
            SendRequest request, 
            AiProvider aiProvider, 
            AppDbContext dbContext,
            CancellationToken cancellationToken = default)
        {
            if (request.Prompt.Length > Constants.MAX_CLIENT_MESSAGE_LENGTH)
                return Results.BadRequest("Prompt length is too long. Max length: " + Constants.MAX_CLIENT_MESSAGE_LENGTH);

            var chatResult = await dbContext.Chats.FirstOrDefaultAsync(x => x.Id == request.ChatId, cancellationToken);
            
            if (chatResult is null)
                return Results.NotFound("Chat not found ");

            var systemPrompt = "Max output tokens: " + Constants.MAX_AI_MESSAGE_LENGTH;
            
            var aiResponse = await aiProvider
                .SendToAi(request.Prompt, chatResult, systemPrompt, request.Base64File, cancellationToken);

            if (aiResponse.IsFailure)
                return aiResponse.Error.ToIResultResponse();
            
            return Results.Ok(Envelope.Ok(aiResponse.Value));
        }
    }
}