using MediatorTest.Billing.Contracts;
using MediatorTest.Mediator;
using MediatorTest.Orders.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorTest.Orders;
 
public static class OrdersExtensions
{
    public static IServiceCollection AddOrdersServices(this IServiceCollection services)
    {
        services.AddScoped<IOrdersService, OrdersHandler>();
        services.AddScoped<IProvider<OrdersRequest, int>, OrdersHandler>();
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
 
public class OrdersHandler(OrdersDbContext context, IMediator mediator) : IOrdersService, IProvider<OrdersRequest,int>
{
    public async Task<OrdersContract> GetOrdersInfo()
    {
        var ordersCount = await context.Orders.CountAsync();

        return new OrdersContract
        {
            OrdersMessage = "This is orders info from Orders Service",
            OrdersCount = ordersCount,
            BillingCount = await mediator.GetResponse(new BillingRequest())
        };
    }

    public async Task AddOrdersInfo()
    {
        var orderEntity = new OrderEntity();
        context.Orders.Add(orderEntity);
        await context.SaveChangesAsync();
    }

    public Task<int> GetOrdersCount()
    {
        return context.Orders.CountAsync();
    }

    public Task<int> GetResponse(OrdersRequest query)
    {
        return GetOrdersCount();
    }
}

public class OrdersContract
{
    public string OrdersMessage { get; set; } = "";
    public int OrdersCount { get; set; } = 0;
    public int BillingCount { get; set; } = 0;
}

public class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options)
{
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
}

public class OrderEntity
{
    public int Id { get; set; }
}