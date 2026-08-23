using HomePal.Application.Features.Notifications.DTOs;
using HomePal.Application.Features.Notifications.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace HomePal.Infrastructure.Notifications;

public class NotificationHubService : INotificationHubService
{
    private readonly IHubContext<NotificationHub, INotificationHubClient> _hubContext;

    public NotificationHubService(IHubContext<NotificationHub, INotificationHubClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationToUserAsync(Guid userId, NotificationResponse notification, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.User(userId.ToString()).GetNotification(notification);
    }
}
