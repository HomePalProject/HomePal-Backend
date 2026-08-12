namespace HomePal.Application.Features.AgentChat.DTOs;

public class AgentChatSessionResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<AgentChatMessageResponse> Messages { get; set; } = new();
}
