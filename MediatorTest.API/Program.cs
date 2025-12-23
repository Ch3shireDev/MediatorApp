using MediatorTest.Billing;
using MediatorTest.Mediator;
using MediatorTest.Orders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddOrdersServices();
builder.Services.AddBillingServices();

builder.Services.AddMediatorServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseOrdersEndpoints();
app.UseBillingEndpoints();
 
app.Run();
 