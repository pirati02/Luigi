using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using User.Api.Feature.Register;

namespace User.Api.Infrastructure;

public static class RabbitMqConnector
{
    public static void ConnectToRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var opt = new RabbitMqServiceOptions();
        var exchangeOptions = new RabbitMqExchangeOptions();
        configuration.GetSection("RabbitMq").Bind(opt);
        configuration.GetSection("RabbitMqExchange").Bind(exchangeOptions);
        
        services.AddRabbitMqServices(opt)
            .AddConsumptionExchange("user.api.exchange", exchangeOptions)
            .AddAsyncMessageHandlerSingleton<RegisterUserEventHandler>("register.routing-key");
    }
}