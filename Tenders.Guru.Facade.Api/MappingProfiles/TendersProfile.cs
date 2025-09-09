using AutoMapper;
using Tenders.Guru.Facade.Api.Services;

namespace Tenders.Guru.Facade.Api.MappingProfiles;

public class TendersProfile : Profile
{
    public TendersProfile()
    {
        CreateMap<Tender, TenderDto>()
            .ForMember(dest => dest.AwardedValueEur, opt => opt.MapFrom(src => 
                string.IsNullOrEmpty(src.AwardedValueEur) ? (decimal?)null : decimal.Parse(src.AwardedValueEur)));

        CreateMap<Purchaser, PurchaserDto>();
        
        CreateMap<TenderType, TenderTypeDto>();
        
        CreateMap<Award, AwardDto>()
            .ForMember(dest => dest.Count, opt => opt.MapFrom(src => int.Parse(src.Count)))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => decimal.Parse(src.Value)));
        
        CreateMap<SupplierInfo, SupplierDto>();
    }
}