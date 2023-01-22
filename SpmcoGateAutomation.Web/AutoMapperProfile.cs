using AutoMapper;
using SpmcoBctsDatabaseMigrator.Domain;

namespace SpmcoGateAutomation.Web
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //CreateMap<(VoyageInfo voyage, ContainersInfo container), ReadDischargeDto>()
            //    .ForMember(dest => dest.CntrNo, opt => opt.MapFrom(src => src.voyage.Manifests.FirstOrDefault().ManifestCntrs.FirstOrDefault().CntrNo))
            //    .ForMember(dest => dest.VoyageId, opt => opt.MapFrom(src => src.voyage.VoyageId))
            //    .ForMember(dest => dest.AgentName, opt => opt.MapFrom(src => src.container.ShippingLine.ShippingLineEnglishName)).ReverseMap();
        }
    }
}
