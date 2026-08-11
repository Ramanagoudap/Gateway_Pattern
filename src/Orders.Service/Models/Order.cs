namespace Orders.Service.Models;

public sealed class Order
{
	public Guid Id { get; set; }
	public string CustomerName { get; set; } = string.Empty;
	public decimal Amount { get; set; }
	public DateTime CreatedAtUtc { get; set; }
	public bool NotificationSent { get; set; }
}

public sealed record CreateOrderRequest(string CustomerName, decimal Amount);

public sealed record CreateOrderResponse(
	Guid Id,
	string CustomerName,
	decimal Amount,
	DateTime CreatedAtUtc,
	bool NotificationSent);
