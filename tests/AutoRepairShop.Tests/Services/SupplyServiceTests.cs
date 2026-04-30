using AutoMapper;
using AutoRepairShop.Application.DTOs.Supply;
using AutoRepairShop.Application.Services;
using AutoRepairShop.Domain.Interfaces.Repositories;
using AutoRepairShop.Domain.Models.Supply;
using Moq;

namespace AutoRepairShop.Tests.Services;

public class SupplyServiceTests
{
    private readonly Mock<ISupplyRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task CreateAsync_WhenRequestIsValid_ShouldPersistSupplyAndMapResponse()
    {
        var request = TestObjectFactory.Create<CreateSupplyRequest>(
            ("Name", "Filtro"),
            ("Price", 45m),
            ("StockQuantity", 10)
        );
        var response = new SupplyResponse();
        _mapperMock
            .Setup(mapper =>
                mapper.Map<SupplyResponse>(
                    It.Is<global::Supply>(supply =>
                        supply.Name == "Filtro" && supply.Price == 45m && supply.StockQuantity == 10
                    )
                )
            )
            .Returns(response);
        var sut = CreateSut();

        var result = await sut.CreateAsync(request);

        Assert.Same(response, result);
        _repositoryMock.Verify(
            repository => repository.AddAsync(It.IsAny<global::Supply>()),
            Times.Once
        );
    }

    [Fact]
    public async Task GetSuppliesInStockAsync_WhenItemsAreProvided_ShouldMapRequestAndReturnRepositoryResult()
    {
        var supplyItems = new List<SupplyItemDto>
        {
            new() { SupplyId = Guid.NewGuid(), Quantity = 2 },
        };
        var mappedItems = new List<SupplyRequestItem>
        {
            new(supplyItems[0].SupplyId, supplyItems[0].Quantity),
        };
        var supplies = new List<global::Supply> { new("Filtro", 45m, 10) };
        _mapperMock
            .Setup(mapper => mapper.Map<List<SupplyRequestItem>>(supplyItems))
            .Returns(mappedItems);
        _repositoryMock
            .Setup(repository => repository.GetSuppliesInStockAsync(mappedItems))
            .ReturnsAsync(supplies);
        var sut = CreateSut();

        var result = await sut.GetSuppliesInStockAsync(supplyItems);

        Assert.Same(supplies, result);
    }

    [Fact]
    public async Task UpdateAsync_WhenSupplyExists_ShouldUpdateEntityAndMapResponse()
    {
        var entity = new global::Supply("Filtro", 45m, 10);
        var request = TestObjectFactory.Create<UpdateSupplyRequest>(
            ("Id", entity.Id),
            ("Name", "Oleo"),
            ("Price", 70m),
            ("StockQuantity", 15)
        );
        var response = new SupplyResponse();
        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(entity.Id))
            .ReturnsAsync(entity);
        _mapperMock.Setup(mapper => mapper.Map<SupplyResponse>(entity)).Returns(response);
        var sut = CreateSut();

        var result = await sut.UpdateAsync(request);

        Assert.Same(response, result);
        Assert.Equal("Oleo", entity.Name);
        Assert.Equal(70m, entity.Price);
        Assert.Equal(15, entity.StockQuantity);
        _repositoryMock.Verify(repository => repository.UpdateAsync(entity), Times.Once);
    }

    [Fact]
    public async Task RestockSuppliesAsync_WhenSuppliesExist_ShouldIncreaseStockAndUpdateEachEntity()
    {
        var first = new global::Supply("Filtro", 45m, 10);
        var second = new global::Supply("Oleo", 70m, 3);
        _repositoryMock.Setup(repository => repository.GetByIdAsync(first.Id)).ReturnsAsync(first);
        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(second.Id))
            .ReturnsAsync(second);
        var sut = CreateSut();

        await sut.RestockSuppliesAsync([(first.Id, 2), (second.Id, 5)]);

        Assert.Equal(12, first.StockQuantity);
        Assert.Equal(8, second.StockQuantity);
        _repositoryMock.Verify(repository => repository.UpdateAsync(first), Times.Once);
        _repositoryMock.Verify(repository => repository.UpdateAsync(second), Times.Once);
    }

    [Fact]
    public async Task RestockSuppliesAsync_WhenSupplyDoesNotExist_ShouldThrow()
    {
        var missingId = Guid.NewGuid();
        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(missingId))
            .ReturnsAsync((global::Supply?)null);
        var sut = CreateSut();

        var action = () => sut.RestockSuppliesAsync([(missingId, 2)]);

        var exception = await Assert.ThrowsAsync<Exception>(action);
        Assert.Equal($"Supply with ID {missingId} not found.", exception.Message);
    }

    private SupplyService CreateSut()
    {
        return new SupplyService(_repositoryMock.Object, _mapperMock.Object);
    }
}
