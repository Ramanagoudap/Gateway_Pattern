using System.Collections.Concurrent;
using Orders.Service.Models;

namespace Orders.Service.Services;

public sealed class OrderStore
{
	private readonly ConcurrentDictionary<Guid, Order> _orders = new();

	public IReadOnlyCollection<Order> GetAll() =>
		_orders.Values.OrderByDescending(o => o.CreatedAtUtc).ToList();

	public Order? GetById(Guid id) =>
		_orders.TryGetValue(id, out var order) ? order : null;

	public Order Add(CreateOrderRequest request)
	{
		var order = new Order
		{
			Id = Guid.NewGuid(),
			CustomerName = request.CustomerName,
			Amount = request.Amount,
			CreatedAtUtc = DateTime.UtcNow,
			NotificationSent = false
		};

		_orders[order.Id] = order;
		return order;
	}

	public void MarkNotificationSent(Guid orderId, bool sent)
	{
		if (_orders.TryGetValue(orderId, out var order))
		{
			order.NotificationSent = sent;
		}
	}
}
