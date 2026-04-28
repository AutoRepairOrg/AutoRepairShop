using AutoRepairShop.Application.DTOs.ServiceOrder.Request;
using AutoRepairShop.Application.DTOs.ServiceOrder.Response;
using AutoRepairShop.Domain.Enums;

namespace AutoRepairShop.Application.Interfaces.Services;

public interface IServiceOrderService
{
    Task CreateServiceOrderAsync(CreateServiceOrderRequest request);

    Task<IEnumerable<GetServiceOrderResponse>> GetAllAsync(ServiceOrderStatus? status);

    Task<GetServiceOrderResponse> GetByIdAsync(Guid serviceOrderId);

    Task AdvanceStatusAsync(Guid serviceOrderId);

    Task ProcessApprovalDecisionAsync(ApprovalDecisionRequest request);
}
