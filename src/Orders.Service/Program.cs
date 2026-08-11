using Orders.Service.Models;
using Orders.Service.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<OrderStore>();
builder.Services.AddHttpClient<INotificationClient, NotificationClient>(client =>
{
	var baseUrl = builder.Configuration["Services:NotificationsBaseUrl"] ?? "http://localhost:5181";
	client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapGet("/api/v1/orders", (OrderStore store) => Results.Ok(store.GetAll()));

app.MapGet("/api/v1/orders/{id:guid}", (Guid id, OrderStore store) =>
{
	var order = store.GetById(id);
	return order is null ? Results.NotFound() : Results.Ok(order);
});

app.MapPost("/api/v1/orders", async (CreateOrderRequest request, OrderStore store, INotificationClient notificationClient, HttpContext context, ILogger<Program> logger) =>
{
	if (string.IsNullOrWhiteSpace(request.CustomerName) || request.Amount <= 0)
	{
		return Results.BadRequest(new { error = "CustomerName is required and Amount must be greater than zero." });
	}

	var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();
	var order = store.Add(request);

	var notificationSent = await notificationClient.SendOrderCreatedAsync(
		order.Id,
		recipient: request.CustomerName,
		message: $"Order {order.Id} created for amount {order.Amount:0.00}",
		correlationId,
		context.RequestAborted);

	store.MarkNotificationSent(order.Id, notificationSent);
	order.NotificationSent = notificationSent;

	logger.LogInformation("Created order {OrderId}. NotificationSent={NotificationSent}. CorrelationId={CorrelationId}", order.Id, notificationSent, correlationId);

	var response = new CreateOrderResponse(order.Id, order.CustomerName, order.Amount, order.CreatedAtUtc, order.NotificationSent);

	return Results.Created($"/api/v1/orders/{order.Id}", response);
});

app.Run();
