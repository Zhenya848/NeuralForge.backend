using AiService.Models.Shared;
using CSharpFunctionalExtensions;

namespace AiService.Models.AiMapping;

public class AiModelConfiguration
{
    
    public float PricePerMillionInputTokens { get; init; }
    public float PricePerMillionOutputTokens { get; init; }
    public float PricePerMillionReadCache { get; init; }
    public float PricePerMillionAudioTokens { get; init; }
    public float PricePerMillionAudioTokensCache { get; init; }
    public float PricePerRequestWebSearch { get; init; }
}