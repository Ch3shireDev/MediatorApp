using System.Net.Http.Json;
using FluentAssertions;
using MediatorTest.Billing;
using MediatorTest.Mediator;
using MediatorTest.Orders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorTest.Tests.Integration;

public class MediatorAppUnitTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly IOrdersService _ordersService;
    private readonly IBillingService _billingService;

    private readonly OrdersDbContext _orders = DbContextBuilder.CreateDbContext<OrdersDbContext>();
    private readonly BillingDbContext _billing = DbContextBuilder.CreateDbContext<BillingDbContext>();

    public MediatorAppUnitTests(WebApplicationFactory<Program> factory)
    {
        var services = new ServiceCollection();
        services.AddOrdersServices();
        services.AddBillingServices();
        services.AddMediatorServices();
        services.AddSingleton(_orders);
        services.AddSingleton(_billing);
        var serviceProvider = services.BuildServiceProvider();
        
        _ordersService = serviceProvider.GetRequiredService<IOrdersService>();
        _billingService = serviceProvider.GetRequiredService<IBillingService>();
    }

    [Fact]
    public async Task OrdersTest()
    {
        // Arrange

        // Act
        var response = await _ordersService.GetOrdersInfo();

        // Assert
        response.Should().NotBeNull();
        response!.OrdersMessage.Should().Be("This is orders info from Orders Service");
    }

    [Fact]
    public async Task AddOrdersTest()
    {
        // Arrange
        // var response1 = await _client.GetFromJsonAsync<OrdersContract>("/orders");
        var response1 =  await _ordersService.GetOrdersInfo();
        response1.Should().NotBeNull();
        response1.OrdersCount.Should().Be(0);

        // Act
        // await _client.PostAsync("/orders", null);
        await _ordersService.AddOrdersInfo();
        // var response2 = await _client.GetFromJsonAsync<OrdersContract>("/orders");
        var response2 = await _ordersService.GetOrdersInfo();

        // Assert
        response2.Should().NotBeNull();
        response2!.OrdersCount.Should().Be(1);
    }
    
    [Fact]
    public async Task BillingTest()
    {
        // Arrange

        // Act
        // var response = await _client.GetFromJsonAsync<BillingContract>("/billing");
        var response = await _billingService.GetBillingInfo();

        // Assert
        response.Should().NotBeNull();
        response!.BillingMessage.Should().Be("This is billing info from Billing Service");
    }

    [Fact]
    public async Task AddBillingTest()
    {
        // Arrange
        // var response1 = await _client.GetFromJsonAsync<BillingContract>("/billing");
        var response1 = await _billingService.GetBillingInfo();
        response1.Should().NotBeNull();
        response1.BillingCount.Should().Be(0);

        // Act
        // await _client.PostAsync("/billing", null);
        await _billingService.AddBillingInfo();

        // Assert
        // var response2 = await _client.GetFromJsonAsync<BillingContract>("/billing");
        var response2 = await _billingService.GetBillingInfo();
        response2.Should().NotBeNull();
        response2.BillingCount.Should().Be(1);
    }
    
    [Fact]
    public async Task MediatorCountsTest()
    {
        // Arrange
        // await _client.PostAsync("/orders", null);
        // await _client.PostAsync("/orders", null);
        // await _client.PostAsync("/billing", null);
        // await _client.PostAsync("/billing", null);
        // await _client.PostAsync("/billing", null);
        await _ordersService.AddOrdersInfo();
        await _ordersService.AddOrdersInfo();
        await _billingService.AddBillingInfo();
        await _billingService.AddBillingInfo();
        await _billingService.AddBillingInfo();

        // Act
        // var ordersResponse = await _client.GetFromJsonAsync<OrdersContract>("/orders");
        var ordersResponse = await _ordersService.GetOrdersInfo();
        // var billingResponse = await _client.GetFromJsonAsync<BillingContract>("/billing");
        var billingResponse = await _billingService.GetBillingInfo();

        // Assert
        ordersResponse.Should().NotBeNull();
        billingResponse.Should().NotBeNull();
        
        ordersResponse.OrdersCount.Should().Be(2);
        ordersResponse.BillingCount.Should().Be(3);
        
        billingResponse.OrdersCount.Should().Be(2);
        billingResponse.BillingCount.Should().Be(3);
    }
}
  