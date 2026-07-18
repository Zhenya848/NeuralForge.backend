using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using AiService.Models.AiClients.Contracts;
using AiService.Models.AiClients.Dtos;
using AiService.Models.AiMapping;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Constants = AiService.Models.Shared.Constants;

namespace AiService.Providers;

public class AiProvider(HttpClient httpClient, IOptionsMonitor<AiSettings> settings)
{
    private const string BaseUrl = "https://openrouter.ai/api/v1";
    
    public async Task<AiResponse> GenerateTextAsync(
        string modelName, 
        List<ChatMessageDto> messages, 
        float temperature,
        CancellationToken ct,
        int? maxTokens = Constants.MAX_AI_MESSAGE_TOKENS)
    {
        var requestBody = new { model = modelName, messages, temperature, max_tokens = maxTokens };
        
        var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.CurrentValue.OpenRouterApiKey);
        request.Content = JsonContent.Create(requestBody);

        var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonNode>(cancellationToken: ct);
            
        string text = json?["choices"]?[0]?["message"]?["content"]?.GetValue<string>() ?? string.Empty;
            
        var usage = json?["usage"];
        int inputTokens = usage?["prompt_tokens"]?.GetValue<int>() ?? 0;
        int outputTokens = usage?["completion_tokens"]?.GetValue<int>() ?? 0;
            
        int cachedTokens = usage?["prompt_tokens_details"]?["cached"]?.GetValue<int>() ?? 0;

        return new AiResponse(text, inputTokens, outputTokens, cachedTokens);
    }
    
    private record OpenAiEmbeddingRequest(
        string Model, 
        List<string> Input);
    
    private record OpenAiEmbeddingResponse(
        List<OpenAiEmbeddingData> Data, 
        string Model);

    private record OpenAiEmbeddingData(
        int Index, 
        float[] Embedding, 
        string Object);
    
    public async Task<List<float[]>> GenerateEmbeddingAsync(List<string> texts, CancellationToken ct)
    {
        var requestBody = new OpenAiEmbeddingRequest(settings.CurrentValue.EmbeddingModel.Name, texts);

        var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/embeddings");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.CurrentValue.OpenRouterApiKey);
        request.Content = JsonContent.Create(requestBody);
        
        var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OpenAiEmbeddingResponse>(cancellationToken: ct);
        var orderedEmbeddings = result.Data
            .OrderBy(d => d.Index)
            .Select(d => d.Embedding)
            .ToList();

        return orderedEmbeddings;
    }
}