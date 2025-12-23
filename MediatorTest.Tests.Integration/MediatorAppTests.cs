using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MediatorTest.Tests.Integration;

public class MediatorAppTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

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
}

internal class OrdersContract
{
    public string OrdersMessage { get; set; } = "";
    public int OrdersCount { get; set; } = 0;
}

internal class BillingContract
{
    public string BillingMessage { get; set; } = "";
    public int BillingCount { get; set; } = 0;
}