using AutoMapper;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Enums;

namespace MarthasLibrary.API.Shared.MappingProfiles;

public class CustomerProfile : Profile
{
  public CustomerProfile()
  {
    CreateMap<Customer, CustomerDetails>()
      .ForMember(dest => dest.PrimaryAddress,
        opt => opt.MapFrom(src => src.Addresses
          .FirstOrDefault(a => a.AddressType == AddressType.Primary)));

    CreateMap<Address, CustomerDetails.AddressDetails>();
  }
}