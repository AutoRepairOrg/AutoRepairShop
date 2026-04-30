using AutoMapper;
using AutoRepairShop.Application.DTOs.Customer;
using AutoRepairShop.Application.Interfaces;
using AutoRepairShop.Application.Services;
using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Interfaces.Repositories;
using Moq;

namespace AutoRepairShop.Tests.Services;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();

    [Fact]
    public async Task CreateAsync_WhenRequestIsValid_ShouldHashPasswordPersistCustomerAndMapResponse()
    {
        var request = new CreateCustomerRequest
        {
            Name = "Maria",
            Document = "12345678909",
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
        _passwordHasherMock
            .Setup(hasher => hasher.Hash(request.Password))
            .Returns("hashed-password");
        _mapperMock
            .Setup(mapper =>
                mapper.Map<CustomerResponse>(
                    It.Is<Customer>(customer =>
                        customer.Name == request.Name
                        && customer.Document.Value == request.Document
                        && customer.Phone == request.Phone
                        && customer.Username == request.Username
                        && customer.Password == "hashed-password"
                    )
                )
            )
            .Returns(response);
        var sut = CreateSut();

        var result = await sut.CreateAsync(request);

        Assert.Same(response, result);
        _repositoryMock.Verify(
            repository =>
                repository.AddAsync(
                    It.Is<Customer>(customer => customer.Password == "hashed-password")
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerDoesNotExist_ShouldThrow()
    {
        var customerId = Guid.NewGuid();
        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(customerId))
            .ReturnsAsync((Customer?)null);
        var sut = CreateSut();

        var action = () => sut.DeleteAsync(customerId);

        var exception = await Assert.ThrowsAsync<Exception>(action);
        Assert.Equal("Customer not found.", exception.Message);
    }

    [Fact]
    public async Task GetAllAsync_WhenCustomersExist_ShouldMapRepositoryResult()
    {
        var customers = new List<Customer>
        {
            new("Maria", Document.Create("12345678909"), "11999999999", "maria", "hash"),
        };
        IEnumerable<CustomerResponse> response =
        [
            new CustomerResponse
            {
                Id = customers[0].Id,
                Name = customers[0].Name,
                Document = customers[0].Document.Value,
                Phone = customers[0].Phone,
                Username = customers[0].Username,
            },
        ];
        _repositoryMock.Setup(repository => repository.GetAllAsync()).ReturnsAsync(customers);
        _mapperMock
            .Setup(mapper => mapper.Map<IEnumerable<CustomerResponse>>(customers))
            .Returns(response);
        var sut = CreateSut();

        var result = await sut.GetAllAsync();

        Assert.Same(response, result);
    }

    [Fact]
    public async Task GetByCpfCnpjAsync_WhenCustomerDoesNotExist_ShouldThrow()
    {
        _repositoryMock
            .Setup(repository => repository.GetByCpfCnpjAsync("12345678909"))
            .ReturnsAsync((Customer?)null);
        var sut = CreateSut();

        Task<Customer> action() => sut.GetByCpfCnpjAsync("12345678909");

        var exception = await Assert.ThrowsAsync<Exception>((Func<Task<Customer>>)action);
        Assert.Equal("Customer not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerExists_ShouldHashPasswordUpdateEntityAndMapResponse()
    {
        var customer = new Customer(
            "Maria",
            Document.Create("12345678909"),
            "11999999999",
            "maria",
            "old-hash"
        );
        var request = new UpdateCustomerRequest
        {
            Id = customer.Id,
            Name = "Maria Silva",
            Phone = "11888888888",
            Username = "maria.silva",
            Password = "new-secret",
        };
        var response = new CustomerResponse
        {
            Id = customer.Id,
            Name = request.Name,
            Document = customer.Document.Value,
            Phone = request.Phone,
            Username = request.Username,
        };
        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(customer.Id))
            .ReturnsAsync(customer);
        _passwordHasherMock.Setup(hasher => hasher.Hash(request.Password)).Returns("new-hash");
        _mapperMock.Setup(mapper => mapper.Map<CustomerResponse>(customer)).Returns(response);
        var sut = CreateSut();

        var result = await sut.UpdateAsync(request);

        Assert.Same(response, result);
        Assert.Equal(request.Name, customer.Name);
        Assert.Equal(request.Phone, customer.Phone);
        Assert.Equal(request.Username, customer.Username);
        Assert.Equal("new-hash", customer.Password);
        _repositoryMock.Verify(repository => repository.UpdateAsync(customer), Times.Once);
    }

    private CustomerService CreateSut()
    {
        return new CustomerService(
            _repositoryMock.Object,
            _mapperMock.Object,
            _passwordHasherMock.Object
        );
    }
}
