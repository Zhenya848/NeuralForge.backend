using CSharpFunctionalExtensions;
using NeuralForge.Domain.Shared.ValueObjects.Ids;

namespace NeuralForge.Domain.Shared;

public class AiModel : Entity<AiModelId>
{
    public string Name { get; }
    public int PricePerMillionInputTokens { get; }
    public int PricePerMillionOutputTokens { get; }
    public int PricePerMillionReadCache { get; }
    public int PricePerMillionWriteCache { get; }
    public int PricePerMillionAudioTokens { get; }
    public int PricePerMillionAudioTokensCache { get; }
    public int PricePerRequestWebSearch { get; }
    
    private AiModel(
        AiModelId id,
        string name, 
        int pricePerMillionInputTokens, 
        int pricePerMillionOutputTokens, 
        int pricePerMillionReadCache, 
        int pricePerMillionWriteCache, 
        int pricePerMillionAudioTokens, 
        int pricePerMillionAudioTokensCache, 
        int pricePerRequestWebSearch) :  base(id)
    {
        Name = name;
        PricePerMillionInputTokens = pricePerMillionInputTokens;
        PricePerMillionOutputTokens = pricePerMillionOutputTokens;
        PricePerMillionReadCache = pricePerMillionReadCache;
        PricePerMillionWriteCache = pricePerMillionWriteCache;
        PricePerMillionAudioTokens = pricePerMillionAudioTokens;
        PricePerMillionAudioTokensCache = pricePerMillionAudioTokensCache;
        PricePerRequestWebSearch = pricePerRequestWebSearch;
    }

    public static Result<AiModel, ErrorList> Create(
        string name,
        int pricePerMillionInputTokens,
        int pricePerMillionOutputTokens,
        int pricePerMillionReadCache,
        int pricePerMillionWriteCache,
        int pricePerMillionAudioTokens,
        int pricePerMillionAudioTokensCache,
        int pricePerRequestWebSearch)
    {
        var errors = new List<Error>();
        
        if (string.IsNullOrWhiteSpace(name))
            errors.Add(Errors.General.ValueIsInvalid("name"));
        
        if (pricePerMillionInputTokens < 0)
            errors.Add(Errors.General.ValueIsInvalid("pricePerMillionInputTokens"));
        
        if (pricePerMillionOutputTokens < 0)
            errors.Add(Errors.General.ValueIsInvalid("pricePerMillionOutputTokens"));
        
        if (pricePerMillionReadCache < 0)
            errors.Add(Errors.General.ValueIsInvalid("pricePerMillionReadCache"));
        
        if (pricePerMillionWriteCache < 0)
            errors.Add(Errors.General.ValueIsInvalid("pricePerMillionWriteCache"));
        
        if (pricePerMillionAudioTokens < 0)
            errors.Add(Errors.General.ValueIsInvalid("pricePerMillionAudioTokens"));
        
        if (pricePerMillionAudioTokensCache < 0)
            errors.Add(Errors.General.ValueIsInvalid("pricePerMillionAudioTokensCache"));
        
        if (pricePerRequestWebSearch < 0)
            errors.Add(Errors.General.ValueIsInvalid("pricePerRequestWebSearch"));

        if (errors.Count > 0)
            return (ErrorList)errors;

        return new AiModel(AiModelId.AddNewId(), name, pricePerMillionInputTokens, pricePerMillionOutputTokens, 
            pricePerMillionReadCache, pricePerMillionWriteCache, pricePerMillionAudioTokens, 
            pricePerMillionAudioTokensCache, pricePerRequestWebSearch);
    }
}