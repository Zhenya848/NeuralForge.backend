namespace NeuralForge.Application.Messages.Send;

public record SendMessageCommand(Guid UserId, Guid ChatId, string Prompt, string Model, string? Base64File = null);