using AutoRepairShop.Api.Controllers;
using AutoRepairShop.Application.DTOs.Service;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AutoRepairShop.Tests.Controllers;

public class ServiceControllerTests
{
    private readonly Mock<IServiceService> _serviceMock = new();

    [Fact]
    public async Task GetAll_WhenServicesExist_ShouldReturnOkWithPayload()
    {
        IEnumerable<ServiceResponse> response = [new ServiceResponse()];
        _serviceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(response);
        var controller = new ServiceController(_serviceMock.Object);

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task GetById_WhenServiceExists_ShouldReturnOkWithPayload()
    {
        var serviceId = Guid.NewGuid();
        var response = new ServiceResponse();
        _serviceMock.Setup(service => service.GetByIdAsync(serviceId)).ReturnsAsync(response);
        var controller = new ServiceController(_serviceMock.Object);

        var result = await controller.GetById(serviceId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task Delete_WhenServiceExists_ShouldReturnOk()
    {
        var serviceId = Guid.NewGuid();
        var controller = new ServiceController(_serviceMock.Object);

        var result = await controller.Delete(serviceId);

        Assert.IsType<OkResult>(result.Result);
        _serviceMock.Verify(service => service.DeleteAsync(serviceId), Times.Once);
    }

    [Fact]
    public async Task Create_WhenServiceSucceeds_ShouldReturnOkWithPayload()
    {
        var request = new CreateServiceRequest();
        var response = new ServiceResponse();
        _serviceMock.Setup(service => service.CreateAsync(request)).ReturnsAsync(response);
        var controller = new ServiceController(_serviceMock.Object);

        var result = await controller.Create(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task Create_WhenDomainExceptionIsThrown_ShouldReturnBadRequest()
    {
        var request = new CreateServiceRequest();
        _serviceMock
            .Setup(service => service.CreateAsync(request))
            .ThrowsAsync(new DomainException("invalid service"));
        var controller = new ServiceController(_serviceMock.Object);

        var result = await controller.Create(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(
            "invalid service",
            ControllerResultValueReader.GetString(badRequestResult.Value, "error")
        );
    }

    [Fact]
    public async Task Update_WhenServiceSucceeds_ShouldReturnOkWithPayload()
    {
        var request = new UpdateServiceRequest();
        var response = new ServiceResponse();
        _serviceMock.Setup(service => service.UpdateAsync(request)).ReturnsAsync(response);
        var controller = new ServiceController(_serviceMock.Object);

        var result = await controller.Update(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task Update_WhenDomainExceptionIsThrown_ShouldReturnBadRequest()
    {
        var request = new UpdateServiceRequest();
        _serviceMock
            .Setup(service => service.UpdateAsync(request))
            .ThrowsAsync(new DomainException("invalid service"));
        var controller = new ServiceController(_serviceMock.Object);

        var result = await controller.Update(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(
            "invalid service",
            ControllerResultValueReader.GetString(badRequestResult.Value, "error")
        );
    }
}
