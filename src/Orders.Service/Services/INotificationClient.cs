namespace Orders.Service.Services;

public interface INotificationClient
{
	Task<bool> SendOrderCreatedAsync(Guid orderId, string recipient, string message, string? correlationId, CancellationToken cancellationToken = default);
}
