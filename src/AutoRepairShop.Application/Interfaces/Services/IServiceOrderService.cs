using AutoRepairShop.Application.DTOs.ServiceOrder.Request;

namespace AutoRepairShop.Application.Interfaces.Services;

public interface IServiceOrderService
{
    Task CreateServiceOrderAsync(CreateServiceOrderRequest request);

    Task StartDiagnosisAsync(Guid serviceOrderId);

    Task RequestApprovalAsync(Guid serviceOrderId);

    Task ApproveAsync(Guid serviceOrderId);

    Task FinishAsync(Guid serviceOrderId);

    Task DeliverAsync(Guid serviceOrderId);
}
