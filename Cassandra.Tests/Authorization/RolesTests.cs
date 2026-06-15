using Cassandra.Application.Authorization;

namespace Cassandra.Tests.Authorization;

public class RolesTests
{
    [Theory]
    [InlineData(Roles.Admin, true)]
    [InlineData(Roles.Sales, true)]
    [InlineData(Roles.Cashier, true)]
    [InlineData(Roles.SuperAdmin, false)]   // SuperAdmin is seeded only, never provisioned
    [InlineData("Bogus", false)]
    public void IsProvisionable_allows_only_dealer_roles(string role, bool expected)
        => Assert.Equal(expected, Roles.IsProvisionable(role));

    [Theory]
    [InlineData(Roles.Sales, true)]
    [InlineData(Roles.Cashier, true)]
    [InlineData(Roles.Admin, false)]        // a dealer Admin cannot mint another Admin
    [InlineData(Roles.SuperAdmin, false)]
    [InlineData("Bogus", false)]
    public void IsDealerStaff_allows_only_sales_and_cashier(string role, bool expected)
        => Assert.Equal(expected, Roles.IsDealerStaff(role));
}
