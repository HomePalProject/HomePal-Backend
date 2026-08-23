using HomePal.Application.Features.Notifications.DTOs;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Notifications.Mappers;

public static class NotificationMapper
{
    public static NotificationResponse ToResponse(this Notification notification)
    {
        return new NotificationResponse
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            ReadAt = notification.ReadAt
        };
    }

    public static IReadOnlyList<NotificationResponse> ToResponseList(this IEnumerable<Notification> notifications)
    {
        return notifications.Select(n => n.ToResponse()).ToList().AsReadOnly();
    }
}
