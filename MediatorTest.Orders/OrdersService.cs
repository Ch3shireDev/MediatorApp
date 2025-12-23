using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorTest.Orders;

public static class OrdersExtensions
{
    public static IServiceCollection AddOrdersServices(this IServiceCollection services)
    {
        services.AddScoped<IOrdersService, OrdersHandler>();
        services.AddDbContext<OrdersDbContext>(options => options.UseInMemoryDatabase("OrdersDb"));

        return services;
    }

    public static WebApplication UseOrdersEndpoints(this WebApplication app)
    {
        app.MapGet("/orders", (IOrdersService ordersService) => ordersService.GetOrdersInfo());
        app.MapPost("/orders", (IOrdersService ordersService) => ordersService.AddOrdersInfo());

        return app;
    }
}

public interface IOrdersService
{
    Task<OrdersContract> GetOrdersInfo();
    Task AddOrdersInfo();
}

public class OrdersHandler(OrdersDbContext context) : IOrdersService
{
    public async Task<OrdersContract> GetOrdersInfo()
    {
        var ordersCount = await context.Orders.CountAsync();

        return new OrdersContract
        {
            OrdersMessage = "This is orders info from Orders Service",
            OrdersCount = ordersCount,
        };
    }

    public async Task AddOrdersInfo()
    {
        var orderEntity = new OrderEntity();
        context.Orders.Add(orderEntity);
        await context.SaveChangesAsync();
    }
}

public class OrdersContract
{
    public string OrdersMessage { get; set; } = "";
    public int OrdersCount { get; set; } = 0;
}

public class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options)
{
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
}

public class OrderEntity
{
    public int Id { get; set; }
}