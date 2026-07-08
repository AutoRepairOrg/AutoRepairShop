using AutoRepairShop.Domain.ValueObjects;

namespace AutoRepairShop.Tests.Domain;

public class ServiceOrderItemValueObjectTests
{
    [Fact]
    public void Constructor_WithValidArguments_ShouldCreateAndCalculateSubtotal()
    {
        var supplyId = Guid.NewGuid();

        var item = new ServiceOrderItem(supplyId, "Filtro", 20m, 3);

        Assert.Equal(supplyId, item.SupplyId);
        Assert.Equal("Filtro", item.SupplyName);
        Assert.Equal(20m, item.UnitPrice);
        Assert.Equal(3, item.Quantity);
        Assert.Equal(60m, item.Subtotal);
    }

    [Fact]
    public void Constructor_WithEmptySupplyId_ShouldThrowArgumentException()
    {
        var action = () => new ServiceOrderItem(Guid.Empty, "Filtro", 20m, 1);

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("SupplyId is required.", exception.Message);
    }

    [Fact]
    public void Constructor_WithInvalidUnitPrice_ShouldThrowArgumentException()
    {
        var action = () => new ServiceOrderItem(Guid.NewGuid(), "Filtro", 0m, 1);

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("Unit price must be greater than zero.", exception.Message);
    }

    [Fact]
    public void Constructor_WithInvalidQuantity_ShouldThrowArgumentException()
    {
        var action = () => new ServiceOrderItem(Guid.NewGuid(), "Filtro", 20m, 0);

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("Quantity must be greater than zero.", exception.Message);
    }
}
