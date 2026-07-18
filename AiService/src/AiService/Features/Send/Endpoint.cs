using AiService.Abstractions;

namespace AiService.Features.Send;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/AI/send", Handler);
    }
    
    private static async Task<IResult> Handler(
        SendCommand command,
        SendHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return Results.BadRequest(Envelope.Error(result.Error));
        
        return Results.Ok(Envelope.Ok(result.Value));
    }
}