using System.Reflection;
using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using Xunit;

public class ModuleBoundariesTests
{
    private static readonly Assembly BillingAssembly = typeof(MediatorTest.Billing.AssemblyMarker).Assembly;
    private static readonly Assembly OrdersAssembly  = typeof(MediatorTest.Orders.AssemblyMarker).Assembly;

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