using AutoRepairShop.Application.DTOs.Service;

namespace AutoRepairShop.Application.Interfaces.Services
{
    public interface IServiceService
        : IBaseService<CreateServiceRequest, UpdateServiceRequest, ServiceResponse>
    {
        Task<List<Service>> GetServicesByIdsAsync(List<Guid> serviceIds);
    }
}
