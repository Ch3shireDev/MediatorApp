using MediatorTest.Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorTest.Billing;

public static class BillingExtensions
{
    public static IServiceCollection AddBillingServices(this IServiceCollection services)
    {
        services.AddScoped<IBillingService, BillingHandler>();
        services.AddScoped<IBillingProvider, BillingHandler>();
        
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

public class BillingHandler(BillingDbContext context, IMediator mediator): IBillingService, IBillingProvider
{
    public async Task<BillingContract> GetBillingInfo()
    {
        var billingCount = await context.Billings.CountAsync();
        
        return new BillingContract
        {
            BillingMessage = "This is billing info from Billing Service",
            BillingCount = billingCount,
            OrdersCount = await mediator.GetOrdersCount(),
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
}

public class BillingContract
{
    public string BillingMessage { get; set; } = "";
    public int BillingCount { get; set; } = 0;
    public int OrdersCount { get; set; } = 0;
}

public class BillingDbContext(DbContextOptions<BillingDbContext> options) : DbContext(options)
{
    public DbSet<BillingEntity> Billings { get; set; }
}

public class BillingEntity
{
    public int Id { get; set; }
}