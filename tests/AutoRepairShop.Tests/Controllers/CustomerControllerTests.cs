using AutoRepairShop.Api.Controllers;
using AutoRepairShop.Application.DTOs.Customer;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AutoRepairShop.Tests.Controllers;

public class CustomerControllerTests
{
    private readonly Mock<ICustomerService> _customerServiceMock = new();

    [Fact]
    public async Task GetAll_WhenCustomersExist_ShouldReturnOkWithPayload()
    {
        IEnumerable<CustomerResponse> response =
        [
            new CustomerResponse
            {
                Id = Guid.NewGuid(),
                Name = "Maria",
                Document = "123",
                Phone = "9999",
                Username = "maria",
            },
        ];
        _customerServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(response);
        var controller = new CustomerController(_customerServiceMock.Object);

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task GetById_WhenCustomerExists_ShouldReturnOkWithPayload()
    {
        var customerId = Guid.NewGuid();
        var response = new CustomerResponse
        {
            Id = customerId,
            Name = "Maria",
            Document = "123",
            Phone = "9999",
            Username = "maria",
        };
        _customerServiceMock
            .Setup(service => service.GetByIdAsync(customerId))
            .ReturnsAsync(response);
        var controller = new CustomerController(_customerServiceMock.Object);

        var result = await controller.GetById(customerId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task Delete_WhenCustomerExists_ShouldReturnOk()
    {
        var customerId = Guid.NewGuid();
        var controller = new CustomerController(_customerServiceMock.Object);

        var result = await controller.Delete(customerId);

        Assert.IsType<OkResult>(result.Result);
        _customerServiceMock.Verify(service => service.DeleteAsync(customerId), Times.Once);
    }

    [Fact]
    public async Task Create_WhenServiceSucceeds_ShouldReturnOkWithPayload()
    {
        var request = new CreateCustomerRequest
        {
            Name = "Maria",
            Document = "12345678900",
            Phone = "11999999999",
            Username = "maria",
            Password = "secret",
        };
        var response = new CustomerResponse
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Document = request.Document,
            Phone = request.Phone,
            Username = request.Username,
        };
        _customerServiceMock.Setup(service => service.CreateAsync(request)).ReturnsAsync(response);
        var controller = new CustomerController(_customerServiceMock.Object);

        var result = await controller.Create(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task Create_WhenDomainExceptionIsThrown_ShouldReturnBadRequest()
    {
        var request = new CreateCustomerRequest
        {
            Name = "Maria",
            Document = "12345678900",
            Phone = "11999999999",
            Username = "maria",
            Password = "secret",
        };
        _customerServiceMock
            .Setup(service => service.CreateAsync(request))
            .ThrowsAsync(new DomainException("invalid customer"));
        var controller = new CustomerController(_customerServiceMock.Object);

        var result = await controller.Create(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(
            "invalid customer",
            ControllerResultValueReader.GetString(badRequestResult.Value, "error")
        );
    }

    [Fact]
    public async Task Update_WhenServiceSucceeds_ShouldReturnOkWithPayload()
    {
        var request = new UpdateCustomerRequest
        {
            Id = Guid.NewGuid(),
            Name = "Maria",
            Phone = "11999999999",
            Username = "maria",
            Password = "secret",
        };
        var response = new CustomerResponse
        {
            Id = request.Id,
            Name = request.Name,
            Document = "12345678900",
            Phone = request.Phone,
            Username = request.Username,
        };
        _customerServiceMock.Setup(service => service.UpdateAsync(request)).ReturnsAsync(response);
        var controller = new CustomerController(_customerServiceMock.Object);

        var result = await controller.Update(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, okResult.Value);
    }

    [Fact]
    public async Task Update_WhenDomainExceptionIsThrown_ShouldReturnBadRequest()
    {
        var request = new UpdateCustomerRequest
        {
            Id = Guid.NewGuid(),
            Name = "Maria",
            Phone = "11999999999",
            Username = "maria",
            Password = "secret",
        };
        _customerServiceMock
            .Setup(service => service.UpdateAsync(request))
            .ThrowsAsync(new DomainException("invalid customer"));
        var controller = new CustomerController(_customerServiceMock.Object);

        var result = await controller.Update(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(
            "invalid customer",
            ControllerResultValueReader.GetString(badRequestResult.Value, "error")
        );
    }
}
