namespace NeuralForge.Application.Fragments.Create;

public record CreateFragmentCommand(
    Guid UserId,
    string Name, 
    string Content, 
    Guid MessageId, 
    Guid? ProjectId = null, 
    Guid? FolderId = null);