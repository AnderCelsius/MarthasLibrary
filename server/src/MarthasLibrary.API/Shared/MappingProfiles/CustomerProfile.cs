using AutoMapper;
using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Enums;

namespace MarthasLibrary.API.Shared.MappingProfiles;

public class CustomerProfile : Profile
{
  public CustomerProfile()
  {
    CreateMap<Customer, CustomerDetails>()
      .ConstructUsing(c => new CustomerDetails(
        c.Id,
        c.Email,
        c.FirstName,
        c.LastName,
        c.Addresses.Where(a => a.AddressType == AddressType.Primary)
          .Select(a => new CustomerDetails.AddressDetails(
            a.Street,
            a.City,
            a.State,
            a.Country,
            a.ZipCode))
          .FirstOrDefault() ?? new CustomerDetails.AddressDetails(
          "", "", "", "", ""),
        c.PhoneNumber,
        c.CreatedAt));

    CreateMap<Address, CustomerDetails.AddressDetails>()
      .ConstructUsing(a => new CustomerDetails.AddressDetails(
        a.Street,
        a.City,
        a.State,
        a.Country,
        a.ZipCode));
  }
}