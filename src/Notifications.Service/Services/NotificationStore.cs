using System.Collections.Concurrent;
using Notifications.Service.Models;

namespace Notifications.Service.Services;

public sealed class NotificationStore
{
	private readonly ConcurrentDictionary<Guid, Notification> _notifications = new();

	public IReadOnlyCollection<Notification> GetAll() =>
		_notifications.Values.OrderByDescending(n => n.CreatedAtUtc).ToList();

	public Notification? GetById(Guid id) =>
		_notifications.TryGetValue(id, out var notification) ? notification : null;

	public Notification Add(CreateNotificationRequest request)
	{
		var notification = new Notification
		{
			Id = Guid.NewGuid(),
			OrderId = request.OrderId,
			Message = request.Message,
			Recipient = request.Recipient,
			CreatedAtUtc = DateTime.UtcNow
		};

		_notifications[notification.Id] = notification;
		return notification;
	}
}
