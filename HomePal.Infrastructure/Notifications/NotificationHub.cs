using HomePal.Application.Features.Notifications.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HomePal.Infrastructure.Notifications;

[Authorize]
public class NotificationHub : Hub<INotificationHubClient>
{
}
