namespace MediatorTest.Mediator;

public static class MediatorExtensions
{
    public static IServiceCollection AddMediatorServices(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();
        
        return services;
    }
}

public interface IMediator
{
    Task<int> GetOrdersCount();
    Task<int> GetBillingCount();
}

public class Mediator: IMediator
{
    public Task<int> GetOrdersCount()
    {
        return Task.FromResult(2);
    }

    public Task<int> GetBillingCount()
    {
        return Task.FromResult(3);
    }
}