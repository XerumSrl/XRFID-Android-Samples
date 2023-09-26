﻿using AutoMapper;
using XRFID.Sample.Common.Dto;
using XRFID.Sample.Server.Entities;
using XRFID.Sample.Server.ViewModels;

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
        CreateMap<Printer, PrinterDto>().ReverseMap().ForAllMembers(opt => opt.AllowNull());
        CreateMap<Label, LabelDto>().ReverseMap().ForAllMembers(opt => opt.AllowNull());

        //repository <-> pagine
        CreateMap<Sku, SkuModel>().ReverseMap().ForAllMembers(opt => opt.AllowNull());
        CreateMap<Product, ProductModel>().ReverseMap().ForAllMembers(opt => opt.AllowNull());
        CreateMap<Label, LabelModel>().ReverseMap().ForAllMembers(opt => opt.AllowNull());
        CreateMap<Printer, PrinterModel>().ReverseMap().ForAllMembers(opt => opt.AllowNull());
        CreateMap<Reader, ReaderModel>().ReverseMap().ForAllMembers(opt => opt.AllowNull());

    }
}
