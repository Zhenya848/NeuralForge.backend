using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Presentation.Extensions;

public static class ApiExtensions
{
    public static ActionResult ToResponse(this Error error) =>
        new ObjectResult(Envelope.Error(error)) { StatusCode = GetStatusCode(error) };

    public static ActionResult ToResponse(this ErrorList errors)
    {
        if (errors.Any() == false)
            return new ObjectResult(null) { StatusCode = StatusCodes.Status500InternalServerError };

        return new ObjectResult(Envelope.Error(errors)) { StatusCode = GetStatusCode(errors) };
    }

    public static ActionResult ToResponse(this object response, int statusCode = StatusCodes.Status200OK) =>
        new ObjectResult(Envelope.Ok(response)) { StatusCode = statusCode };
    
    private static int GetStatusCode(ErrorList errors)
    {
        var statusCode = errors.Count() > 1 
            ? StatusCodes.Status500InternalServerError
            : errors.First().ErrorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Required => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Failure => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };
        
        return statusCode;
    }
}