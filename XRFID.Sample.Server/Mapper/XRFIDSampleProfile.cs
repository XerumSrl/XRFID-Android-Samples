using AutoMapper;
using XRFID.Sample.Common.Dto;
using XRFID.Sample.Server.Entities;

namespace XRFID.Sample.Server.Mapper;

public class XRFIDSampleProfile : Profile
{
    public XRFIDSampleProfile()
    {
        CreateMap<Movement, MovementDto>().ReverseMap().ForAllMembers(opt => opt.AllowNull());

        CreateMap<MovementItem, MovementItemDto>().ReverseMap().ForAllMembers(opt => opt.AllowNull());

        CreateMap<LoadingUnit, LoadingUnitDto>().ReverseMap().ForAllMembers(opt => opt.AllowNull());

        CreateMap<LoadingUnitItem, LoadingUnitItemDto>().ReverseMap().ForAllMembers(opt => opt.AllowNull());

        CreateMap<Reader, ReaderDto>().ReverseMap().ForAllMembers(opt => opt.AllowNull());

        CreateMap<Product, ProductDto>().ReverseMap().ForAllMembers(opt => opt.AllowNull());
    }
}
