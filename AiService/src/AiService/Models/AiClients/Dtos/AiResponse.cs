namespace AiService.Models.AiClients.Dtos;

public record AiResponse(string Content, int InputTokens, int OutputTokens, int CachedInputTokens);