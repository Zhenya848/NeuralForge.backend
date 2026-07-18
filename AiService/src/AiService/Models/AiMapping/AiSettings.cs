namespace AiService.Models.AiMapping;

public sealed class AiSettings
{
    public Dictionary<string, AiModelConfiguration> AiModels { get; init; } = [];
    public EmbeddingModelConfiguration EmbeddingModel { get; init; }
    public string AiModelForSummarization { get; init; } = "";
    
    public string OpenRouterApiKey { get; init; } = "";
}