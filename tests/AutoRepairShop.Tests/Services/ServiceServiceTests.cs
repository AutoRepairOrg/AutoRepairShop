using AutoMapper;
using AutoRepairShop.Application.DTOs.Service;
using AutoRepairShop.Application.Services;
using AutoRepairShop.Domain.Interfaces.Repositories;
using Moq;

namespace AutoRepairShop.Tests.Services;

public class ServiceServiceTests
{
    private readonly Mock<IServiceRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task CreateAsync_WhenRequestIsValid_ShouldPersistServiceAndMapResponse()
    {
        var request = TestObjectFactory.Create<CreateServiceRequest>(
            ("Name", "Troca de oleo"),
            ("Description", "Descricao"),
            ("Price", 150m)
        );
        var response = new ServiceResponse();
        _mapperMock
            .Setup(mapper =>
                mapper.Map<ServiceResponse>(
                    It.Is<global::Service>(service =>
                        service.Name == "Troca de oleo"
                        && service.Description == "Descricao"
                        && service.Price == 150m
                    )
                )
            )
            .Returns(response);
        var sut = CreateSut();

        var result = await sut.CreateAsync(request);

        Assert.Same(response, result);
        _repositoryMock.Verify(
            repository => repository.AddAsync(It.IsAny<global::Service>()),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteAsync_WhenServiceDoesNotExist_ShouldThrow()
    {
        var serviceId = Guid.NewGuid();
        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(serviceId))
            .ReturnsAsync((global::Service?)null);
        var sut = CreateSut();

        var action = () => sut.DeleteAsync(serviceId);

        var exception = await Assert.ThrowsAsync<Exception>(action);
        Assert.Equal("Service not found.", exception.Message);
    }

    [Fact]
    public async Task GetAllAsync_WhenServicesExist_ShouldMapRepositoryResult()
    {
        var services = new List<global::Service> { new("Troca de oleo", "Descricao", 150m) };
        IEnumerable<ServiceResponse> response = [new ServiceResponse()];
        _repositoryMock.Setup(repository => repository.GetAllAsync()).ReturnsAsync(services);
        _mapperMock
            .Setup(mapper => mapper.Map<IEnumerable<ServiceResponse>>(services))
            .Returns(response);
        var sut = CreateSut();

        var result = await sut.GetAllAsync();

        Assert.Same(response, result);
    }

    [Fact]
    public async Task GetServicesByIdsAsync_WhenRepositoryReturnsServices_ShouldReturnSameList()
    {
        var ids = new List<Guid> { Guid.NewGuid() };
        var services = new List<global::Service> { new("Troca de oleo", "Descricao", 150m) };
        _repositoryMock
            .Setup(repository => repository.GetServicesByIdsAsync(ids))
            .ReturnsAsync(services);
        var sut = CreateSut();

        var result = await sut.GetServicesByIdsAsync(ids);

        Assert.Same(services, result);
    }

    [Fact]
    public async Task UpdateAsync_WhenServiceExists_ShouldUpdateEntityAndMapResponse()
    {
        var entity = new global::Service("Troca de oleo", "Descricao", 150m);
        var request = TestObjectFactory.Create<UpdateServiceRequest>(
            ("Id", entity.Id),
            ("Name", "Balanceamento"),
            ("Description", "Descricao atualizada"),
            ("Price", 220m)
        );
        var response = new ServiceResponse();
        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(entity.Id))
            .ReturnsAsync(entity);
        _mapperMock.Setup(mapper => mapper.Map<ServiceResponse>(entity)).Returns(response);
        var sut = CreateSut();

        var result = await sut.UpdateAsync(request);

        Assert.Same(response, result);
        Assert.Equal("Balanceamento", entity.Name);
        Assert.Equal("Descricao atualizada", entity.Description);
        Assert.Equal(220m, entity.Price);
        _repositoryMock.Verify(repository => repository.UpdateAsync(entity), Times.Once);
    }

    private ServiceService CreateSut()
    {
        return new ServiceService(_repositoryMock.Object, _mapperMock.Object);
    }
}
