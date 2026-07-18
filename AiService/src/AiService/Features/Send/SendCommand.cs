namespace AiService.Features.Send;

public record SendCommand(Guid UserId, Guid ChatId, string Prompt, string Model, string Base64File);