using CSharpFunctionalExtensions;
using NeuralForge.Domain.Chats.ValueObjects.Ids;
using NeuralForge.Domain.Shared;
using NeuralForge.Domain.Shared.ValueObjects.Ids;

namespace NeuralForge.Domain.Chats;

public class AiMessage : Message
{
    public string ModelName { get; }
    public AiModelId ModelId { get; }

    private AiMessage(MessageId id, ChatId chatId, int serialNumber, string value, DateTime createdAt, AiModel model) 
        : base(id, chatId, serialNumber, value, createdAt)
    {
        ModelId = model.Id;
        ModelName = model.Name;
    }
    
    public static Result<AiMessage, ErrorList> Create(ChatId chatId, int serialNumber, string value, AiModel model)
    {
        var now = DateTime.UtcNow;
        var validateResult = ValidateBaseParameters(serialNumber, value, now);
        
        if (validateResult.IsFailure)
            return validateResult.Error;

        return new AiMessage(MessageId.AddNewId(), chatId, serialNumber, value, now, model);
    }
}