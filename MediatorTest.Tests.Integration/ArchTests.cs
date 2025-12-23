using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using Assembly = System.Reflection.Assembly;

namespace MediatorTest.Tests.Integration;

public class ArchTests
{
    private static readonly Assembly BillingAssembly = typeof(MediatorTest.Billing.BillingHandler).Assembly;
    private static readonly Assembly OrdersAssembly  = typeof(MediatorTest.Orders.OrdersHandler).Assembly;

    private static readonly Architecture Architecture =
        new ArchLoader()
            .LoadAssemblies(BillingAssembly, OrdersAssembly)
            .Build();

    [Fact]
    public void Billing_must_not_depend_on_Orders()
    {
        var billingTypes = ArchRuleDefinition.Types().That().ResideInAssembly(BillingAssembly);
        var rule = billingTypes.Should().NotDependOnAny(ArchRuleDefinition.Types().That().ResideInAssembly(OrdersAssembly));

        rule.Check(Architecture);
    }

    [Fact]
    public void Orders_must_not_depend_on_Billing()
    {
        var ordersTypes = ArchRuleDefinition.Types().That().ResideInAssembly(OrdersAssembly);
        var rule = ordersTypes.Should().NotDependOnAny(ArchRuleDefinition.Types().That().ResideInAssembly(BillingAssembly));

        rule.Check(Architecture);
    }
}