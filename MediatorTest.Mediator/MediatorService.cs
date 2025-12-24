namespace MediatorTest.Mediator;

public static class MediatorExtensions
{
    public static IServiceCollection AddMediatorServices(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();

        return services;
    }
}

public interface IOrdersProvider
{
    Task<int> GetOrdersCount();
}

public interface IBillingProvider
{
    Task<int> GetBillingCount();
}

public interface IMediator : IOrdersProvider, IBillingProvider;

public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public Task<int> GetOrdersCount()
    {
        return serviceProvider.GetRequiredService<IOrdersProvider>().GetOrdersCount();
    }

    public Task<int> GetBillingCount()
    {
        return serviceProvider.GetRequiredService<IBillingProvider>().GetBillingCount();
    }
}
