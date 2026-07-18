namespace NeuralForge.Domain.Shared;

public class Constants
{
    public const string DefaultProjectName = "Default";
    public const string DefaultFolderName = "Default";
    public const string DefaultChatName = "New chat";
    
    public class LoadLimits
    {
        public const int MaxProjects = 20;
        public const int MaxFoldersPerProject = 100;
        public const int MaxChatsTotal = 1000;
        public const int MaxChatsPerFolder = 20;
    }
}