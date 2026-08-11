using ApiGateway.Extensions;
using ApiGateway.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGatewayServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCorrelationId();
app.UseIdempotencyPolicy();
app.UseResponseCachePolicy();
app.MapReverseProxy();
app.Run();
