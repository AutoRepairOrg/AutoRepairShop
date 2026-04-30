using AutoRepairShop.Api.Controllers;
using AutoRepairShop.Application.DTOs.Supply;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AutoRepairShop.Tests.Controllers;

public class SupplyControllerTests
{
    private readonly Mock<ISupplyService> _serviceMock = new();

    [Fact]
    public async Task GetAll_WhenSuppliesExist_ShouldReturnOkWithPayload()
    {
        IEnumerable<SupplyResponse> response = [new SupplyResponse()];
        _serviceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(response);
        var controller = new SupplyController(_serviceMock.Object);

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task GetById_WhenSupplyExists_ShouldReturnOkWithPayload()
    {
        var supplyId = Guid.NewGuid();
        var response = new SupplyResponse();
        _serviceMock.Setup(service => service.GetByIdAsync(supplyId)).ReturnsAsync(response);
        var controller = new SupplyController(_serviceMock.Object);

        var result = await controller.GetById(supplyId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task Delete_WhenSupplyExists_ShouldReturnOk()
    {
        var supplyId = Guid.NewGuid();
        var controller = new SupplyController(_serviceMock.Object);

        var result = await controller.Delete(supplyId);

        Assert.IsType<OkResult>(result.Result);
        _serviceMock.Verify(service => service.DeleteAsync(supplyId), Times.Once);
    }

    [Fact]
    public async Task Create_WhenServiceSucceeds_ShouldReturnOkWithPayload()
    {
        var request = new CreateSupplyRequest();
        var response = new SupplyResponse();
        _serviceMock.Setup(service => service.CreateAsync(request)).ReturnsAsync(response);
        var controller = new SupplyController(_serviceMock.Object);

        var result = await controller.Create(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task Create_WhenDomainExceptionIsThrown_ShouldReturnBadRequest()
    {
        var request = new CreateSupplyRequest();
        _serviceMock
            .Setup(service => service.CreateAsync(request))
            .ThrowsAsync(new DomainException("invalid supply"));
        var controller = new SupplyController(_serviceMock.Object);

        var result = await controller.Create(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(
            "invalid supply",
            ControllerResultValueReader.GetString(badRequestResult.Value, "error")
        );
    }

    [Fact]
    public async Task Update_WhenServiceSucceeds_ShouldReturnOkWithPayload()
    {
        var request = new UpdateSupplyRequest();
        var response = new SupplyResponse();
        _serviceMock.Setup(service => service.UpdateAsync(request)).ReturnsAsync(response);
        var controller = new SupplyController(_serviceMock.Object);

        var result = await controller.Update(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task Update_WhenDomainExceptionIsThrown_ShouldReturnBadRequest()
    {
        var request = new UpdateSupplyRequest();
        _serviceMock
            .Setup(service => service.UpdateAsync(request))
            .ThrowsAsync(new DomainException("invalid supply"));
        var controller = new SupplyController(_serviceMock.Object);

        var result = await controller.Update(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(
            "invalid supply",
            ControllerResultValueReader.GetString(badRequestResult.Value, "error")
        );
    }
}
