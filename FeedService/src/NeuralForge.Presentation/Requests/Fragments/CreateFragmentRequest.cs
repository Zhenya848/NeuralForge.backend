namespace NeuralForge.Presentation.Requests.Fragments;

public record CreateFragmentRequest(
    string Name, 
    string Content, 
    Guid MessageId, 
    Guid? ProjectId = null, 
    Guid? FolderId = null);