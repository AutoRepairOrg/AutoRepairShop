using AutoMapper;
using AutoRepairShop.Application.DTOs.Customer;
using AutoRepairShop.Application.DTOs.Service;
using AutoRepairShop.Application.DTOs.ServiceOrder.Response;
using AutoRepairShop.Application.DTOs.Supply;
using AutoRepairShop.Application.DTOs.Vehicle;
using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Entities.ServiceOrder;
using AutoRepairShop.Domain.Models.Supply;

namespace AutoRepairShop.Application.Mapping
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Customer, CustomerResponse>()
                .ForMember(dest => dest.Document, opt => opt.MapFrom(src => src.Document.Value));

            CreateMap<Vehicle, VehicleResponse>()
                .ForMember(dest => dest.Plate, opt => opt.MapFrom(src => src.Plate.Value));

            CreateMap<Service, ServiceResponse>();
            CreateMap<Supply, SupplyResponse>();
            CreateMap<SupplyItemDto, SupplyRequestItem>();

            // ServiceOrder mappings
            CreateMap<ServiceOrder, GetServiceOrderResponse>()
                .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.Services))
                .ForMember(dest => dest.Supplies, opt => opt.MapFrom(src => src.Supplies))
                .ForMember(dest => dest.History, opt => opt.MapFrom(src => src.History));

            CreateMap<ServiceOrderService, ServiceOrderItemResponse>()
                .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.ServiceId));

            CreateMap<ServiceOrderSupply, ServiceOrderSupplyItemResponse>()
                .ForMember(dest => dest.SupplyId, opt => opt.MapFrom(src => src.SupplyId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<ServiceOrderHistory, ServiceOrderHistoryResponse>();
        }
    }
}
