using AutoMapper;
using Tenders.Guru.Facade.Api.Models.DTOs;
using Tenders.Guru.Facade.Api.Models.TenderApiModels;

namespace Tenders.Guru.Facade.Api.MappingProfiles;

public class TendersProfile : Profile
{
    public TendersProfile()
    {
        CreateMap<Tender, TenderDto>()
            .ForMember(dest => dest.AmountInEur, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.AwardedValueEur) ? (decimal?)null : decimal.Parse(src.AwardedValueEur)))
            .ForMember(dest => dest.Suppliers, opt => opt.MapFrom(src =>
                src.Awarded.SelectMany(award => award.Suppliers).ToList()));
        
        CreateMap<SupplierInfo, SupplierDto>();
    }
}