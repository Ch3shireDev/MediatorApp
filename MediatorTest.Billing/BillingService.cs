using MediatorTest.Billing.Contracts;
using MediatorTest.Mediator;
using MediatorTest.Orders.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MediatorTest.Billing;

public static class BillingExtensions
{
    public static IServiceCollection AddBillingServices(this IServiceCollection services)
    {
        services.AddScoped<IBillingService, BillingHandler>();
        services.AddScoped<IProvider<BillingRequest, int>, BillingHandler>();

        services.AddDbContext<BillingDbContext>(options => options.UseInMemoryDatabase("BillingDb"));

        return services;
    }

    public static WebApplication UseBillingEndpoints(this WebApplication app)
    {
        app.MapGet("/billing", (IBillingService billingService) => billingService.GetBillingInfo());
        app.MapPost("/billing", (IBillingService billingService) => billingService.AddBillingInfo());

        return app;
    }
}

public interface IBillingService
{
    Task<BillingContract> GetBillingInfo();
    Task AddBillingInfo();
}

public class BillingHandler(BillingDbContext context, IMediator mediator)
    : IBillingService, IProvider<BillingRequest, int>
{
    public async Task<BillingContract> GetBillingInfo()
    {
        var billingCount = await GetBillingCount();

        return new BillingContract
        {
            BillingMessage = "This is billing info from Billing Service",
            BillingCount = billingCount,
            OrdersCount = await mediator.GetResponse(new OrdersRequest()),
        };
    }

    public async Task AddBillingInfo()
    {
        var billingEntity = new BillingEntity();
        context.Billings.Add(billingEntity);
        await context.SaveChangesAsync();
    }

    public Task<int> GetBillingCount()
    {
        return context.Billings.CountAsync();
    }

    public Task<int> GetResponse(BillingRequest query)
    {
        return GetBillingCount();
    }
}

public class BillingContract
{
    public string BillingMessage { get; set; } = "";
    public int BillingCount { get; set; }
    public int OrdersCount { get; set; }
}

public class BillingDbContext(DbContextOptions<BillingDbContext> options) : DbContext(options)
{
    public DbSet<BillingEntity> Billings { get; set; }
}

public class BillingEntity
{
    public int Id { get; set; }
}