using AutoRepairShop.Domain.ValueObjects;

namespace AutoRepairShop.Tests.Domain;

public class VehiclePlateValueObjectTests
{
    [Fact]
    public void Create_WithOldPatternPlate_ShouldNormalizeAndCreate()
    {
        var plate = VehiclePlate.Create("abc-1234");

        Assert.Equal("ABC1234", plate.Value);
    }

    [Fact]
    public void Create_WithMercosulPatternPlate_ShouldNormalizeAndCreate()
    {
        var plate = VehiclePlate.Create("abc1d23");

        Assert.Equal("ABC1D23", plate.Value);
    }

    [Fact]
    public void Create_WithInvalidPattern_ShouldThrowArgumentException()
    {
        var action = () => VehiclePlate.Create("INVALID");

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("Invalid vehicle plate", exception.Message);
    }
}
