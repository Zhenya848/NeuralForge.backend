using CSharpFunctionalExtensions;
using NeuralForge.Domain.Chats.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Domain.Chats;

public class ClientMessage : Message
{
    private ClientMessage(MessageId id, ChatId chatId, int serialNumber, string value, DateTime createdAt) 
        : base(id, chatId, serialNumber, value, createdAt)
    {
    }
    
    public static Result<ClientMessage, ErrorList> Create(ChatId chatId, int serialNumber, string value)
    {
        var now = DateTime.UtcNow;
        var validateResult = ValidateBaseParameters(serialNumber, value, now);
        
        if (validateResult.IsFailure)
            return validateResult.Error;

        return new ClientMessage(MessageId.AddNewId(), chatId, serialNumber, value, now);
    }
}