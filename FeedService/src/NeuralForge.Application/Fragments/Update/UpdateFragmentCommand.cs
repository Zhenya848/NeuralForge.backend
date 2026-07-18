namespace NeuralForge.Application.Fragments.Update;

public record UpdateFragmentCommand(Guid UserId, Guid FragmentId, string Name, Guid? FolderId = null);