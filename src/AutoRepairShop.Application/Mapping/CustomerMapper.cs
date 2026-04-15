using AutoMapper;
using AutoRepairShop.Application.DTOs.Customer;

namespace AutoRepairShop.Application.Mapping
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerResponse>()
                .ForMember(dest => dest.Document,
                  opt => opt.MapFrom(src => src.Document.Value));

            CreateMap<UpdateCustomerRequest, Customer>()
                .ForMember(dest => dest.Document,
                  opt => opt.MapFrom(src => Document.Create(src.Document)));
        }
    }
}
