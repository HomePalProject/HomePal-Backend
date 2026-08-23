using HomePal.Application.Features.Notifications.DTOs;
using HomePal.Domain.Enums;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Notifications.Interfaces;

public interface INotificationService
{
    Task<Result<IReadOnlyList<NotificationResponse>>> GetMyNotificationsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<UnreadCountResponse>> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<NotificationResponse>> MarkAsReadAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default);
    Task<Result> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result> DeleteNotificationAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default);
    Task<Result<NotificationResponse>> SendNotificationAsync(Guid userId, string title, string message, NotificationType type = NotificationType.General, CancellationToken cancellationToken = default);
}
