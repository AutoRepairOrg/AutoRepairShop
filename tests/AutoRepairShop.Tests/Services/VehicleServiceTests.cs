using AutoMapper;
using AutoRepairShop.Application.DTOs.Vehicle;
using AutoRepairShop.Application.Services;
using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Interfaces.Repositories;
using Moq;

namespace AutoRepairShop.Tests.Services;

public class VehicleServiceTests
{
    private readonly Mock<IVehicleRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task CreateAsync_WhenRequestIsValid_ShouldPersistVehicleAndMapResponse()
    {
        var request = TestObjectFactory.Create<CreateVehicleRequest>(
            ("CustomerId", Guid.NewGuid()),
            ("Plate", new VehiclePlateDto { Value = "ABC1234" }),
            ("Brand", "Ford"),
            ("Model", "Ka"),
            ("Year", 2022)
        );
        var response = new VehicleResponse();
        _mapperMock
            .Setup(mapper =>
                mapper.Map<VehicleResponse>(
                    It.Is<Vehicle>(vehicle =>
                        vehicle.CustomerId == request.CustomerId
                        && vehicle.Plate.Value == "ABC1234"
                        && vehicle.Brand == "Ford"
                        && vehicle.Model == "Ka"
                        && vehicle.Year == 2022
                    )
                )
            )
            .Returns(response);
        var sut = CreateSut();

        var result = await sut.CreateAsync(request);

        Assert.Same(response, result);
        _repositoryMock.Verify(repository => repository.AddAsync(It.IsAny<Vehicle>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenVehicleExists_ShouldUpdateEntityAndMapResponse()
    {
        var entity = new Vehicle(
            Guid.NewGuid(),
            AutoRepairShop.Domain.ValueObjects.VehiclePlate.Create("ABC1234"),
            "Ford",
            "Ka",
            2022
        );
        var request = TestObjectFactory.Create<UpdateVehicleRequest>(
            ("Id", entity.Id),
            ("CustomerId", entity.CustomerId),
            ("Plate", new VehiclePlateDto { Value = "ABC1234" }),
            ("Brand", "Fiat"),
            ("Model", "Uno"),
            ("Year", 2023)
        );
        var response = new VehicleResponse();
        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(entity.Id))
            .ReturnsAsync(entity);
        _mapperMock.Setup(mapper => mapper.Map<VehicleResponse>(entity)).Returns(response);
        var sut = CreateSut();

        var result = await sut.UpdateAsync(request);

        Assert.Same(response, result);
        Assert.Equal("Fiat", entity.Brand);
        Assert.Equal("Uno", entity.Model);
        Assert.Equal(2023, entity.Year);
        _repositoryMock.Verify(repository => repository.UpdateAsync(entity), Times.Once);
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenVehicleAlreadyExists_ShouldReturnExistingVehicle()
    {
        var existing = new Vehicle(
            Guid.NewGuid(),
            AutoRepairShop.Domain.ValueObjects.VehiclePlate.Create("ABC1234"),
            "Ford",
            "Ka",
            2022
        );
        var request = new VehicleDto
        {
            Plate = "ABC1234",
            Brand = "Ford",
            Model = "Ka",
            Year = 2022,
        };
        _repositoryMock
            .Setup(repository => repository.GetByPlateAsync(request.Plate))
            .ReturnsAsync(existing);
        var sut = CreateSut();

        var result = await sut.GetOrCreateAsync(request, Guid.NewGuid());

        Assert.Same(existing, result);
        _repositoryMock.Verify(repository => repository.AddAsync(It.IsAny<Vehicle>()), Times.Never);
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenVehicleDoesNotExist_ShouldCreatePersistAndReturnVehicle()
    {
        var customerId = Guid.NewGuid();
        var request = new VehicleDto
        {
            Plate = "ABC1234",
            Brand = "Ford",
            Model = "Ka",
            Year = 2022,
        };
        _repositoryMock
            .Setup(repository => repository.GetByPlateAsync(request.Plate))
            .ReturnsAsync((Vehicle?)null);
        var sut = CreateSut();

        var result = await sut.GetOrCreateAsync(request, customerId);

        Assert.Equal(customerId, result.CustomerId);
        Assert.Equal(request.Plate, result.Plate.Value);
        _repositoryMock.Verify(
            repository =>
                repository.AddAsync(
                    It.Is<Vehicle>(vehicle =>
                        vehicle.CustomerId == customerId && vehicle.Plate.Value == request.Plate
                    )
                ),
            Times.Once
        );
    }

    private VehicleService CreateSut()
    {
        return new VehicleService(_repositoryMock.Object, _mapperMock.Object);
    }
}
