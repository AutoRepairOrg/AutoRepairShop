namespace AutoRepairShop.Application.Interfaces.Services;
public interface IServiceOrderService
{
    Task<Guid> CreateAsync(Guid customerId, Guid vehicleId,Guid serviceId);

    Task AddSupplyAsync(
        Guid serviceOrderId, 
        Guid supplyId, 
        string supplyName,
        int quantity,
        decimal unitPrice,
        decimal servicePrice);

    Task StartDiagnosisAsync(Guid serviceOrderId);

    Task RequestApprovalAsync(Guid serviceOrderId);

    Task ApproveAsync(Guid serviceOrderId);

    Task FinishAsync(Guid serviceOrderId);

    Task DeliverAsync(Guid serviceOrderId);
}