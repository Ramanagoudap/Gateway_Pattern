using Notifications.Service.Models;
using Notifications.Service.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<NotificationStore>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapGet("/api/v1/notifications", (NotificationStore store, HttpContext context, ILogger<Program> logger) =>
{
	var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();
	logger.LogInformation("Listing notifications. CorrelationId={CorrelationId}", correlationId);
	return Results.Ok(store.GetAll());
});

app.MapGet("/api/v1/notifications/{id:guid}", (Guid id, NotificationStore store, HttpContext context, ILogger<Program> logger) =>
{
	var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();
	logger.LogInformation("Get notification {Id}. CorrelationId={CorrelationId}", id, correlationId);

	var notification = store.GetById(id);
	return notification is null ? Results.NotFound() : Results.Ok(notification);
});

app.MapPost("/api/v1/notifications", (CreateNotificationRequest request, NotificationStore store, HttpContext context, ILogger<Program> logger) =>
{
	var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();

	if (string.IsNullOrWhiteSpace(request.Message) || string.IsNullOrWhiteSpace(request.Recipient))
	{
		return Results.BadRequest(new { error = "Message and Recipient are required." });
	}

	var notification = store.Add(request);
	logger.LogInformation(
		"Created notification {Id} for order {OrderId}. CorrelationId={CorrelationId}",
		notification.Id,
		notification.OrderId,
		correlationId);

	return Results.Created($"/api/v1/notifications/{notification.Id}", notification);
});

app.Run();
