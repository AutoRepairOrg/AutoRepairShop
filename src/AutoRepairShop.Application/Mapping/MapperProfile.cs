using AutoMapper;
using AutoRepairShop.Application.DTOs.Customer;
using AutoRepairShop.Application.DTOs.Service;
using AutoRepairShop.Application.DTOs.Supply;
using AutoRepairShop.Application.DTOs.Vehicle;
using AutoRepairShop.Domain.Entities;
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
        }
    }
}
