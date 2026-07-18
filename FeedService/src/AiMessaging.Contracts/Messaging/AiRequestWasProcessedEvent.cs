namespace AiMessaging.Contracts.Messaging;

public record AiRequestWasProcessedEvent(Guid ChatId, string ClientMessage, string AiMessage, string AiModel);