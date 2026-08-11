namespace Notifications.Service.Models;

public sealed class Notification
{
	public Guid Id { get; set; }
	public Guid? OrderId { get; set; }
	public string Message { get; set; } = string.Empty;
	public string Recipient { get; set; } = string.Empty;
	public DateTime CreatedAtUtc { get; set; }
}

public sealed record CreateNotificationRequest(Guid? OrderId, string Message, string Recipient);
