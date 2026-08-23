using HomePal.Application.Features.Notifications.DTOs;

namespace HomePal.Application.Features.Notifications.Interfaces;

public interface INotificationHubService
{
    Task SendNotificationToUserAsync(Guid userId, NotificationResponse notification, CancellationToken cancellationToken = default);
}
