using CSharpFunctionalExtensions;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Domain.Chats.ValueObjects;

public record AiModel
{
    public string Name { get; }
    
    private AiModel(string name)
    {
        Name = name;
    }

    public static Result<AiModel, ErrorList> Create(string name)
    {
        var errors = new List<Error>();
        
        if (string.IsNullOrWhiteSpace(name))
            errors.Add(Errors.General.ValueIsInvalid("name"));

        if (errors.Count > 0)
            return (ErrorList)errors;

        return new AiModel(name);
    }
}