namespace NeuralForge.Presentation.Requests.Fragments;

public record UpdateFragmentRequest(string Name, Guid? FolderId = null);