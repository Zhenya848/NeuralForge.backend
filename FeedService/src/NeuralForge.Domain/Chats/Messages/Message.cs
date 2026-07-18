using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using NeuralForge.Domain.Chats.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Domain.Chats.Messages;

public abstract class Message : Shared.Entity<MessageId>
{
    public ChatId ChatId { get; }
    public int SerialNumber { get; }
    public string Value { get; }
    public DateTime CreatedAt { get; }
    public bool IsBookmarked;

    [JsonConstructor]
    protected Message(MessageId id) : base(id)
    {
        
    }
    
    protected Message(MessageId id, ChatId chatId, int serialNumber, string value, DateTime createdAt) : base(id)
    {
        ChatId = chatId;
        SerialNumber = serialNumber;
        Value = value;
        CreatedAt = createdAt;
    }

    protected static UnitResult<ErrorList> ValidateBaseParameters(int serialNumber, string value, DateTime createdAt)
    {
        var errors = new List<Error>();
        
        if (string.IsNullOrWhiteSpace(value))
            errors.Add(Errors.General.ValueIsInvalid("value"));
        
        if (createdAt > DateTime.UtcNow)
            errors.Add(Errors.General.ValueIsInvalid("createdAt"));
        
        if (serialNumber < 1)
            errors.Add(Errors.General.ValueIsInvalid("serialNumber"));

        if (errors.Count > 0)
            return (ErrorList)errors;

        return Result.Success<ErrorList>();
    }
}