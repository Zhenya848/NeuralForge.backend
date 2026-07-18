namespace NeuralForge.Presentation.Requests.Projects;

public record CreateProjectRequest(string Name, string Description, int EmojiId, int ColorId);