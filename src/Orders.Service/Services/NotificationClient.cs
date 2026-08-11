using System.Net.Http.Json;

namespace Orders.Service.Services;

public sealed class NotificationClient : INotificationClient
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<NotificationClient> _logger;

	public NotificationClient(HttpClient httpClient, ILogger<NotificationClient> logger)
	{
		_httpClient = httpClient;
		_logger = logger;
	}

	public async Task<bool> SendOrderCreatedAsync(
		Guid orderId,
		string recipient,
		string message,
		string? correlationId,
		CancellationToken cancellationToken = default)
	{
		try
		{
			using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/notifications");
			if (!string.IsNullOrWhiteSpace(correlationId))
			{
				request.Headers.TryAddWithoutValidation("X-Correlation-ID", correlationId);
			}

			request.Content = JsonContent.Create(new
			{
				orderId,
				message,
				recipient
			});

			using var response = await _httpClient.SendAsync(request, cancellationToken);
			if (response.IsSuccessStatusCode)
			{
				return true;
			}

			_logger.LogWarning(
				"Notification service returned {StatusCode} for order {OrderId}",
				(int)response.StatusCode,
				orderId);
			return false;
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Failed to notify for order {OrderId}", orderId);
			return false;
		}
	}
}
