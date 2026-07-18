namespace NeuralForge.Application.ChatFolders.Delete;

public record DeleteChatFolderCommand(Guid UserId, Guid FolderId);