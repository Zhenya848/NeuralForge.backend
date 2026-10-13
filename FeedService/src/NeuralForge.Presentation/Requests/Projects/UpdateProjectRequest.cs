namespace NeuralForge.Presentation.Requests.Projects;

public record UpdateProjectRequest(string Name, string Description, int EmojiId, int ColorId);