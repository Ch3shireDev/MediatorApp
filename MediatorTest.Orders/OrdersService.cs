using Microsoft.Extensions.DependencyInjection;

namespace MediatorTest.Orders;

public static class OrdersExtensions
{
    public static IServiceCollection AddOrdersServices(this IServiceCollection services)
    {
        services.AddScoped<IOrdersService, OrdersHandler>();
        return services;
    }
    
    public static WebApplication UseOrdersEndpoints(this WebApplication app)
    {
        app.MapGet("/orders", (IOrdersService ordersService) => ordersService.GetOrdersInfo());
        
        return app;
    }
}

public interface IOrdersService
{
    OrdersContract GetOrdersInfo();
}

public class OrdersHandler: IOrdersService
{
    public OrdersContract GetOrdersInfo()
    {
        return new OrdersContract
        {
            OrdersMessage = "This is orders info from Orders Service"
        };
    }
}

public class OrdersContract
{
    public string OrdersMessage { get; set; } = "";
}