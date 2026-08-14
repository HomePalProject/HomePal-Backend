using HomePal.Domain.Common;

namespace HomePal.Domain.Entities;

public class AgentChatSession : BaseAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public string? SessionStateJson { get; set; }


    public ICollection<AgentChatMessage> Messages { get; set; } = new List<AgentChatMessage>();
}
