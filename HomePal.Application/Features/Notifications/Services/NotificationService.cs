using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Notifications.DTOs;
using HomePal.Application.Features.Notifications.Interfaces;
using HomePal.Application.Features.Notifications.Mappers;
using HomePal.Domain.Entities;
using HomePal.Domain.Enums;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Notifications.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationHubService _hubService;

    public NotificationService(
        IUnitOfWork unitOfWork,
        INotificationHubService hubService)
    {
        _unitOfWork = unitOfWork;
        _hubService = hubService;
    }

    public async Task<Result<IReadOnlyList<NotificationResponse>>> GetMyNotificationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var notifications = await _unitOfWork.Notifications.GetByUserIdAsync(userId, cancellationToken);
        return Result<IReadOnlyList<NotificationResponse>>.Ok(
            notifications.ToResponseList(),
            SuccessMessages.Notification.GetNotifications);
    }

    public async Task<Result<UnreadCountResponse>> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var count = await _unitOfWork.Notifications.GetUnreadCountAsync(userId, cancellationToken);
        return Result<UnreadCountResponse>.Ok(
            new UnreadCountResponse { Count = count },
            SuccessMessages.Notification.GetUnreadCount);
    }

    public async Task<Result<NotificationResponse>> MarkAsReadAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAndUserIdAsync(notificationId, userId, cancellationToken);
        if (notification == null)
        {
            return Result<NotificationResponse>.Fail(ErrorMessages.Notification.NotificationNotFound, ResultStatus.NotFound);
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _unitOfWork.Notifications.Update(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result<NotificationResponse>.Ok(
            notification.ToResponse(),
            SuccessMessages.Notification.MarkAsRead);
    }

    public async Task<Result> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Notifications.MarkAllAsReadAsync(userId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Notification.MarkAllAsRead);
    }

    public async Task<Result> DeleteNotificationAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAndUserIdAsync(notificationId, userId, cancellationToken);
        if (notification == null)
        {
            return Result.Fail(ErrorMessages.Notification.NotificationNotFound, ResultStatus.NotFound);
        }

        _unitOfWork.Notifications.Remove(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Notification.DeleteNotification);
    }

    public async Task<Result<NotificationResponse>> SendNotificationAsync(
        Guid userId,
        string title,
        string message,
        NotificationType type = NotificationType.General,
        CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return Result<NotificationResponse>.Fail(ErrorMessages.Notification.UserNotFound, ResultStatus.NotFound);
        }

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = notification.ToResponse();

        await _hubService.SendNotificationToUserAsync(userId, response, cancellationToken);

        return Result<NotificationResponse>.Ok(response, SuccessMessages.General, ResultStatus.Created);
    }
}
