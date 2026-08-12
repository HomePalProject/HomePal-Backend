namespace HomePal.Domain.Entities;

public class AgentChatMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ChatSessionId { get; set; }
    public AgentChatSession ChatSession { get; set; } = null!;

    public string Role { get; set; } = "user"; // user | assistant | tool | system
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
