namespace TaskManager.Models
{
    public enum Priority
    {
        Low,      // basse
        Medium,   // moyenne
        High,     // haute
        Critical  // critique
    }

    public enum Status
    {
        ToDo,       // à faire
        InProgress, // en cours
        Done,       // terminée
        Cancelled   // annulée
    }
} 