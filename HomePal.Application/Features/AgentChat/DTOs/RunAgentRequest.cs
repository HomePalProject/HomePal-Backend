namespace HomePal.Application.Features.AgentChat.DTOs;

public class RunAgentRequest
{
    public string? Message { get; set; }
    public bool? Approved { get; set; }
    public string? ToolCallId { get; set; }
}
