using AutoRepairShop.Domain.Entities.ServiceOrder;
using AutoRepairShop.Domain.Enums;
using AutoRepairShop.Domain.Exceptions;

namespace AutoRepairShop.Tests.Domain;

public class ServiceOrderAggregateTests
{
    [Fact]
    public void StartDiagnosis_WhenStatusIsReceived_ShouldMoveToInDiagnosis()
    {
        var sut = new ServiceOrder();

        sut.StartDiagnosis();

        Assert.Equal(ServiceOrderStatus.InDiagnosis, sut.Status);
    }

    [Fact]
    public void RequestApproval_WhenStatusIsInDiagnosis_ShouldMoveToWaitingApproval()
    {
        var sut = new ServiceOrder();
        sut.StartDiagnosis();

        sut.RequestApproval();

        Assert.Equal(ServiceOrderStatus.WaitingApproval, sut.Status);
    }

    [Fact]
    public void Approve_WhenStatusIsWaitingApproval_ShouldMoveToInExecutionAndSetStartedAt()
    {
        var sut = new ServiceOrder();
        sut.StartDiagnosis();
        sut.RequestApproval();

        sut.Approve();

        Assert.Equal(ServiceOrderStatus.InExecution, sut.Status);
        Assert.NotNull(sut.StartedAt);
    }

    [Fact]
    public void Finish_WhenStatusIsInExecution_ShouldMoveToFinishedAndSetFinishedAt()
    {
        var sut = new ServiceOrder();
        sut.StartDiagnosis();
        sut.RequestApproval();
        sut.Approve();

        sut.Finish();

        Assert.Equal(ServiceOrderStatus.Finished, sut.Status);
        Assert.NotNull(sut.FinishedAt);
    }

    [Fact]
    public void Deliver_WhenStatusIsFinished_ShouldMoveToDelivered()
    {
        var sut = new ServiceOrder();
        sut.StartDiagnosis();
        sut.RequestApproval();
        sut.Approve();
        sut.Finish();

        sut.Deliver();

        Assert.Equal(ServiceOrderStatus.Delivered, sut.Status);
    }

    [Fact]
    public void Approve_WhenStatusIsNotWaitingApproval_ShouldThrowDomainException()
    {
        var sut = new ServiceOrder();

        var action = () => sut.Approve();

        var exception = Assert.Throws<DomainException>(action);
        Assert.Equal("Service order must be awaiting approval.", exception.Message);
    }

    [Fact]
    public void Reject_WhenStatusIsNotWaitingApproval_ShouldThrowDomainException()
    {
        var sut = new ServiceOrder();

        var action = () => sut.Reject();

        var exception = Assert.Throws<DomainException>(action);
        Assert.Equal("Service order must be waiting approval to be canceled.", exception.Message);
    }

    [Fact]
    public void ReplaceServices_ShouldRemovePreviousServicesAndKeepOnlyProvidedOnes()
    {
        var sut = new ServiceOrder();
        var first = Guid.NewGuid();
        var second = Guid.NewGuid();

        sut.AddService(Guid.NewGuid());
        sut.ReplaceServices([first, second]);

        Assert.Equal(2, sut.Services.Count);
        Assert.Contains(sut.Services, service => service.ServiceId == first);
        Assert.Contains(sut.Services, service => service.ServiceId == second);
    }

    [Fact]
    public void ReplaceSupplies_ShouldRemovePreviousSuppliesAndKeepOnlyProvidedOnes()
    {
        var sut = new ServiceOrder();
        var first = Guid.NewGuid();
        var second = Guid.NewGuid();

        sut.AddSupply(Guid.NewGuid(), 1);
        sut.ReplaceSupplies([(first, 2), (second, 4)]);

        Assert.Equal(2, sut.Supplies.Count);
        Assert.Contains(sut.Supplies, supply => supply.SupplyId == first && supply.Quantity == 2);
        Assert.Contains(sut.Supplies, supply => supply.SupplyId == second && supply.Quantity == 4);
    }
}
