using Microsoft.Extensions.Options;

namespace AiService.Models.AiMapping;

public class AiRegistry
{
    private Dictionary<string, AiModelConfiguration> _models;
    public AiModelConfiguration AiModelForSummarization { get; private set; }
    public string AiModelForSummarizationName { get; private set; }

    public AiRegistry(IOptionsMonitor<AiSettings> monitor)
    {
        _models = monitor.CurrentValue.AiModels;
        
        if (!monitor.CurrentValue.AiModels.TryGetValue(monitor.CurrentValue.AiModelForSummarization, out var model))
            throw new KeyNotFoundException("Ai model for summarization not found");

        UpdateSummarizer(monitor.CurrentValue);
        
        monitor.OnChange(settings =>
        {
            _models = settings.AiModels;
            UpdateSummarizer(settings);
        });
    }
    
    private void UpdateSummarizer(AiSettings settings)
    {
        if (settings.AiModels.TryGetValue(settings.AiModelForSummarization, out var model) == false)
            throw new KeyNotFoundException($"Ai model for summarization '{settings.AiModelForSummarization}' not found");
        
        AiModelForSummarization = model;
        AiModelForSummarizationName = settings.AiModelForSummarization;
    }

    public bool TryGet(string name, out AiModelConfiguration? model) =>
        _models.TryGetValue(name, out model);
}