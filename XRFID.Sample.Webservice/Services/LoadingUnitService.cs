﻿using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using XRFID.Sample.Common.Dto;
using XRFID.Sample.Webservice.Database;
using XRFID.Sample.Webservice.Entities;
using XRFID.Sample.Webservice.Repositories;

namespace XRFID.Sample.Webservice.Services;

public class LoadingUnitService
{
    private readonly LoadingUnitRepository repository;
    private readonly LoadingUnitItemRepository itemRepository;
    private readonly IMapper mapper;
    private readonly UnitOfWork uowk;

    public LoadingUnitService(LoadingUnitRepository repository, LoadingUnitItemRepository itemRepository, IMapper mapper, UnitOfWork uowk)
    {
        this.repository = repository;
        this.itemRepository = itemRepository;
        this.mapper = mapper;
        this.uowk = uowk;
    }

    public async Task<List<LoadingUnitDto>> GetAsync()
    {
        List<LoadingUnit> result = await repository.GetAsync();
        if (result.IsNullOrEmpty())
        {
            throw new KeyNotFoundException();
        }
        return mapper.Map<List<LoadingUnitDto>>(result);
    }

    public async Task<LoadingUnitDto> GetByReferenceAsync(string reference)
    {
        List<LoadingUnit> resultList = await repository.GetAsync(q => q.OrderReference != null && q.OrderReference == reference);
        if (resultList.IsNullOrEmpty())
        {
            throw new KeyNotFoundException();
        }

        LoadingUnit? result = resultList.FirstOrDefault() ?? throw new KeyNotFoundException();
        return mapper.Map<LoadingUnitDto>(result);
    }

    public async Task<LoadingUnitDto> GetWithItemsAsync(string reference)
    {
        List<LoadingUnit> resultList = await repository.GetAsync(q => (q.OrderReference != null && q.OrderReference == reference)
                                                                   || (q.Reference != null && q.Reference == reference));
        if (resultList.IsNullOrEmpty())
        {
            throw new KeyNotFoundException();
        }

        LoadingUnit? result = resultList.FirstOrDefault() ?? throw new KeyNotFoundException();
        List<LoadingUnitItem> itemResult = await itemRepository.GetAsync(q => q.LoadingUnitId == result.Id);

        if (result.LoadingUnitItems is null)
        {
            result.LoadingUnitItems = new List<LoadingUnitItem>();
        }
        result.LoadingUnitItems = itemResult;

        return mapper.Map<LoadingUnitDto>(result);
    }

    public async Task<LoadingUnitDto> CreateAsync(LoadingUnitDto loadingUnitDto)
    {
        LoadingUnit result = await repository.CreateAsync(mapper.Map<LoadingUnit>(loadingUnitDto));

        await uowk.SaveAsync();

        return mapper.Map<LoadingUnitDto>(result);
    }
}
