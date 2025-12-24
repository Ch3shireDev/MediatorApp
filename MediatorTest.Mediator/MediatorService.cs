namespace MediatorTest.Mediator;

public static class MediatorExtensions
{
    public static IServiceCollection AddMediatorServices(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();

        return services;
    }
}

public interface IQuery<TResponse>;

public interface IMediator
{
    Task<TResponse> GetResponse<TResponse>(IQuery<TResponse> query);
}

public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    Task<TResponse> GetResponse(TQuery query);
}

public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public Task<TResponse> GetResponse<TResponse>(IQuery<TResponse> query)
    {
        var providerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        dynamic provider = serviceProvider.GetRequiredService(providerType);
        return provider.GetResponse((dynamic)query);
    }
}
