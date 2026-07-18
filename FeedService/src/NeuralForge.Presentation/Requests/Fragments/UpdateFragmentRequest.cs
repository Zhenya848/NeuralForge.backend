namespace NeuralForge.Presentation.Requests.Fragments;

public record UpdateFragmentRequest(Guid FragmentId, string Name, Guid? FolderId = null);