using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorTest.Billing;

public static class BillingExtensions
{
    public static IServiceCollection AddBillingServices(this IServiceCollection services)
    {
        services.AddScoped<IBillingService, BillingHandler>();
        return services;
    }
    
    public static WebApplication UseBillingEndpoints(this WebApplication app)
    {
        app.MapGet("/billing", (IBillingService billingService) => billingService.GetBillingInfo());
        
        return app;
    }
}

public interface IBillingService
{
    BillingContract GetBillingInfo();
}

public class BillingHandler: IBillingService
{
    public BillingContract GetBillingInfo()
    {
        return new BillingContract
        {
            BillingMessage = "This is billing info from Billing Service"
        };
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