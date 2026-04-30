using AutoRepairShop.Api.Controllers;
using AutoRepairShop.Application.DTOs.Vehicle;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AutoRepairShop.Tests.Controllers;

public class VehicleControllerTests
{
    private readonly Mock<IVehicleService> _vehicleServiceMock = new();

    [Fact]
    public async Task GetAll_WhenVehiclesExist_ShouldReturnOkWithPayload()
    {
        IEnumerable<VehicleResponse> response = [new VehicleResponse()];
        _vehicleServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(response);
        var controller = new VehicleController(_vehicleServiceMock.Object);

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task GetById_WhenVehicleExists_ShouldReturnOkWithPayload()
    {
        var vehicleId = Guid.NewGuid();
        var response = new VehicleResponse();
        _vehicleServiceMock
            .Setup(service => service.GetByIdAsync(vehicleId))
            .ReturnsAsync(response);
        var controller = new VehicleController(_vehicleServiceMock.Object);

        var result = await controller.GetById(vehicleId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task Delete_WhenVehicleExists_ShouldReturnOk()
    {
        var vehicleId = Guid.NewGuid();
        var controller = new VehicleController(_vehicleServiceMock.Object);

        var result = await controller.Delete(vehicleId);

        Assert.IsType<OkResult>(result.Result);
        _vehicleServiceMock.Verify(service => service.DeleteAsync(vehicleId), Times.Once);
    }

    [Fact]
    public async Task Create_WhenServiceSucceeds_ShouldReturnOkWithPayload()
    {
        var request = new CreateVehicleRequest();
        var response = new VehicleResponse();
        _vehicleServiceMock.Setup(service => service.CreateAsync(request)).ReturnsAsync(response);
        var controller = new VehicleController(_vehicleServiceMock.Object);

        var result = await controller.Create(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task Create_WhenDomainExceptionIsThrown_ShouldReturnBadRequest()
    {
        var request = new CreateVehicleRequest();
        _vehicleServiceMock
            .Setup(service => service.CreateAsync(request))
            .ThrowsAsync(new DomainException("invalid vehicle"));
        var controller = new VehicleController(_vehicleServiceMock.Object);

        var result = await controller.Create(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(
            "invalid vehicle",
            ControllerResultValueReader.GetString(badRequestResult.Value, "error")
        );
    }

    [Fact]
    public async Task Update_WhenServiceSucceeds_ShouldReturnOkWithPayload()
    {
        var request = new UpdateVehicleRequest();
        var response = new VehicleResponse();
        _vehicleServiceMock.Setup(service => service.UpdateAsync(request)).ReturnsAsync(response);
        var controller = new VehicleController(_vehicleServiceMock.Object);

        var result = await controller.Update(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task Update_WhenDomainExceptionIsThrown_ShouldReturnBadRequest()
    {
        var request = new UpdateVehicleRequest();
        _vehicleServiceMock
            .Setup(service => service.UpdateAsync(request))
            .ThrowsAsync(new DomainException("invalid vehicle"));
        var controller = new VehicleController(_vehicleServiceMock.Object);

        var result = await controller.Update(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(
            "invalid vehicle",
            ControllerResultValueReader.GetString(badRequestResult.Value, "error")
        );
    }
}
