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
        var response =await  _client.GetFromJsonAsync<OrdersContract>("/orders");
        
        // Assert
        response.Should().NotBeNull();
        response!.OrdersMessage.Should().Be("This is orders info from Orders Service");
    }
    
    [Fact]
    public async Task BillingTest()
    {
        // Arrange
        
        // Act
        var response =await  _client.GetFromJsonAsync<BillingContract>("/billing");
        
        // Assert
        response.Should().NotBeNull();
        response!.BillingMessage.Should().Be("This is billing info from Billing Service");
    }
}

internal class OrdersContract
{
    public string OrdersMessage { get; set; } = "";
}

internal class BillingContract
{
    public string BillingMessage { get; set; } = "";
}