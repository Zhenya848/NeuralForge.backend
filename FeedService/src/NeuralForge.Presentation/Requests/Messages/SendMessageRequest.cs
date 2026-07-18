namespace NeuralForge.Presentation.Requests.Messages;

public record SendMessageRequest(Guid ChatId, string Prompt, string Model, string? Base64File = null);