using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using NeuralForge.Domain.Chats.ValueObjects;
using NeuralForge.Domain.Chats.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Domain.Chats.Messages;

public class AiMessage : Message
{
    public AiModel AiModel { get; }

    [JsonConstructor]
    private AiMessage(MessageId id) : base(id)
    {
        
    }
    
    private AiMessage(MessageId id, ChatId chatId, int serialNumber, string value, DateTime createdAt, AiModel model) 
        : base(id, chatId, serialNumber, value, createdAt)
    {
        AiModel = model;
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