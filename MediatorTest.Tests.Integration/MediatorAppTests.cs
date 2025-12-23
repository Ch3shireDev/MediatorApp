using System.Net.Http.Json;
using FluentAssertions;
using MediatorTest.Billing;
using MediatorTest.Orders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorTest.Tests.Integration;

public class MediatorAppTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    private readonly OrdersDbContext _orders = DbContextBuilder.CreateDbContext<OrdersDbContext>();
    private readonly BillingDbContext _billing = DbContextBuilder.CreateDbContext<BillingDbContext>();

    public MediatorAppTests(WebApplicationFactory<Program> factory)
    {
        _client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(_orders);
                    services.AddSingleton(_billing);
                });
            })
            
            .CreateClient();
    }

    [Fact]
    public async Task OrdersTest()
    {
        // Arrange

        // Act
        var response = await _client.GetFromJsonAsync<OrdersContract>("/orders");

        // Assert
        response.Should().NotBeNull();
        response!.OrdersMessage.Should().Be("This is orders info from Orders Service");
    }

    [Fact]
    public async Task AddOrdersTest()
    {
        // Arrange
        var response1 = await _client.GetFromJsonAsync<OrdersContract>("/orders");
        response1.Should().NotBeNull();
        response1.OrdersCount.Should().Be(0);

        // Act
        await _client.PostAsync("/orders", null);
        var response2 = await _client.GetFromJsonAsync<OrdersContract>("/orders");

        // Assert
        response2.Should().NotBeNull();
        response2!.OrdersCount.Should().Be(1);
    }
    
    [Fact]
    public async Task BillingTest()
    {
        // Arrange

        // Act
        var response = await _client.GetFromJsonAsync<BillingContract>("/billing");

        // Assert
        response.Should().NotBeNull();
        response!.BillingMessage.Should().Be("This is billing info from Billing Service");
    }

    [Fact]
    public async Task AddBillingTest()
    {
        // Arrange
        var response1 = await _client.GetFromJsonAsync<BillingContract>("/billing");
        response1.Should().NotBeNull();
        response1.BillingCount.Should().Be(0);

        // Act
        await _client.PostAsync("/billing", null);

        // Assert
        var response2 = await _client.GetFromJsonAsync<BillingContract>("/billing");
        response2.Should().NotBeNull();
        response2.BillingCount.Should().Be(1);
    }
    
    [Fact]
    public async Task MediatorCountsTest()
    {
        // Arrange
        await _client.PostAsync("/orders", null);
        await _client.PostAsync("/orders", null);
        await _client.PostAsync("/billing", null);
        await _client.PostAsync("/billing", null);
        await _client.PostAsync("/billing", null);

        // Act
        var ordersResponse = await _client.GetFromJsonAsync<OrdersContract>("/orders");
        var billingResponse = await _client.GetFromJsonAsync<BillingContract>("/billing");

        // Assert
        ordersResponse.Should().NotBeNull();
        billingResponse.Should().NotBeNull();
        
        ordersResponse.OrdersCount.Should().Be(2);
        ordersResponse.BillingCount.Should().Be(3);
        
        billingResponse.OrdersCount.Should().Be(2);
        billingResponse.BillingCount.Should().Be(3);
        
    }
}

internal class OrdersContract
{
    public string OrdersMessage { get; set; } = "";
    public int OrdersCount { get; set; } = 0;
    public int BillingCount { get; set; } = 0;
}

internal class BillingContract
{
    public string BillingMessage { get; set; } = "";
    public int BillingCount { get; set; } = 0;
    public int OrdersCount { get; set; } = 0;
}

public static class DbContextBuilder
{
    public static T CreateDbContext<T>() where T : DbContext
    {
        var options = new DbContextOptionsBuilder<T>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        return (T)Activator.CreateInstance(typeof(T), options)!;
    }
}