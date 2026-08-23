using HomePal.Domain.Common;
using HomePal.Domain.Enums;

namespace HomePal.Domain.Entities;

public class Notification : BaseAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.General;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
