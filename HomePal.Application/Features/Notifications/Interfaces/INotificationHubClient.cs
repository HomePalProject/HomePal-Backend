using HomePal.Application.Features.Notifications.DTOs;

namespace HomePal.Application.Features.Notifications.Interfaces;

public interface INotificationHubClient
{
    Task GetNotification(NotificationResponse notification);
}
