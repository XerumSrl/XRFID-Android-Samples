using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using XRFID.Sample.Common.Dto;
using XRFID.Sample.Server.Database;
using XRFID.Sample.Server.Entities;
using XRFID.Sample.Server.Repositories;

namespace XRFID.Sample.Server.Services;

public class LoadingUnitItemService
{
    private readonly LoadingUnitItemRepository repository;
    private readonly IMapper mapper;
    private readonly UnitOfWork uowk;

    public LoadingUnitItemService(LoadingUnitItemRepository repository, IMapper mapper, UnitOfWork uowk)
    {
        this.repository = repository;
        this.mapper = mapper;
        this.uowk = uowk;
    }

    public async Task<List<LoadingUnitItemDto>> GetByLoadingUnitIdAsync(Guid luId)
    {
        List<LoadingUnitItem> result = await repository.GetAsync(q => q.LoadingUnitId == luId);
        if (result.IsNullOrEmpty())
        {
            throw new KeyNotFoundException("Resource not found");
        }
        return mapper.Map<List<LoadingUnitItemDto>>(result);
    }

    public async Task<LoadingUnitItemDto> CreateAsync(LoadingUnitItemDto loadingUnitDto)
    {
        LoadingUnitItem result = await repository.CreateAsync(mapper.Map<LoadingUnitItem>(loadingUnitDto));

        await uowk.SaveAsync();

        return mapper.Map<LoadingUnitItemDto>(result);
    }
}
