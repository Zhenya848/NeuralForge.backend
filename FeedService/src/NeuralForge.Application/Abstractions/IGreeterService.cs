using CSharpFunctionalExtensions;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.Abstractions;

public interface IGreeterService
{
    public Task<Result<string, ErrorList>> SendToAi(
        Guid userId,
        Guid chatId,
        string prompt,
        string aiModel,
        CancellationToken cancellationToken = default,
        string? base64File = null);
}