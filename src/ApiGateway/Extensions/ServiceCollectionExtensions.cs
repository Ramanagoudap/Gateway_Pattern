using ApiGateway.Configuration;
using ApiGateway.Middleware;
using ApiGateway.Services.Caching;
using ApiGateway.Services.Idempotency;
using StackExchange.Redis;
using Yarp.ReverseProxy.Transforms;

namespace ApiGateway.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGatewayServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<GatewayOptions>(configuration.GetSection(GatewayOptions.SectionName));

		var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";

		services.AddStackExchangeRedisCache(options =>
		{
			options.Configuration = redisConnectionString;
			options.InstanceName = "ApiGateway_";
		});

		services.AddSingleton<IConnectionMultiplexer>(_ =>
		{
			var config = ConfigurationOptions.Parse(redisConnectionString);
			config.AbortOnConnectFail = false;
			return ConnectionMultiplexer.Connect(config);
		});

		services.AddSingleton<IResponseCacheService, ResponseCacheService>();
		services.AddSingleton<IIdempotencyService, IdempotencyService>();

		services.AddReverseProxy()
			.LoadFromConfig(configuration.GetSection("ReverseProxy"))
			.AddTransforms(builderContext =>
			{
				builderContext.AddRequestTransform(transformContext =>
				{
					var correlationId = transformContext.HttpContext.Items[CorrelationIdMiddleware.ItemKey]?.ToString();
					if (!string.IsNullOrWhiteSpace(correlationId))
					{
						transformContext.ProxyRequest.Headers.Remove(CorrelationIdMiddleware.HeaderName);
						transformContext.ProxyRequest.Headers.TryAddWithoutValidation(
							CorrelationIdMiddleware.HeaderName,
							correlationId);
					}

					return ValueTask.CompletedTask;
				});
			});

		return services;
	}
}
