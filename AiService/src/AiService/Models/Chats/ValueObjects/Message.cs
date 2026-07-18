using System.Text.Json.Serialization;
using AiService.Models.Shared;
using CSharpFunctionalExtensions;

namespace AiService.Models.Chats.ValueObjects;

public record Message
{
    public int SerialNumber { get; private set; }
    public string Value { get; }
    public Role Role { get; }
    public string? Model { get; }
    
    [JsonConstructor]
    private Message(int serialNumber, string value, Role role,  string? model = null)
    {
        SerialNumber = serialNumber;
        Value = value;
        Role = role;
        Model = model;
    }

    public static Result<Message, Error> Create(string value, Role role, string? model = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Errors.General.ValueIsRequired("value");
        
        if (model is not null && string.IsNullOrWhiteSpace(model))
            return Errors.General.ValueIsRequired("model");
        
        return new Message(0, value, role, model);
    }

    internal UnitResult<Error> ChangeSerialNumber(int serialNumber)
    {
        if (serialNumber < 1)
            return Errors.General.ValueIsInvalid("serial number");
        
        SerialNumber = serialNumber;

        return Result.Success<Error>();
    }
}