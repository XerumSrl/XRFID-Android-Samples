using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using XRFID.Demo.Common.Dto;
using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;
using XRFID.Demo.Server.Repositories;

namespace XRFID.Demo.Server.Services;

public class LabelService
{
    private readonly LabelRepository repository;
    private readonly IMapper mapper;
    private readonly UnitOfWork uowk;

    public LabelService(LabelRepository repository, IMapper mapper, UnitOfWork uowk)
    {
        this.repository = repository;
        this.mapper = mapper;
        this.uowk = uowk;
    }

    public async Task<List<LabelDto>> GetAsync()
    {
        List<Label> labels = await repository.GetAsync();

        if (!labels.Any())
        {
            throw new KeyNotFoundException("Resource not found");
        }

        return mapper.Map<List<LabelDto>>(labels);
    }

    public async Task<LabelDto> GetByIdAsync(Guid id)
    {
        List<Label> resultList = await repository.GetAsync(q => q.Id == id);
        if (resultList.IsNullOrEmpty())
        {
            throw new KeyNotFoundException("Resource not found");
        }

        Label? result = resultList.FirstOrDefault() ?? throw new KeyNotFoundException("Resource not found");
        return mapper.Map<LabelDto>(result);
    }

    public async Task<LabelDto> GetByNameAsync(string name)
    {
        List<Label> resultList = await repository.GetAsync(q => q.Name == name);
        if (resultList.IsNullOrEmpty())
        {
            throw new KeyNotFoundException("Resource not found");
        }

        Label? result = resultList.OrderByDescending(o => o.Version).FirstOrDefault() ?? throw new KeyNotFoundException("Resource not found");
        return mapper.Map<LabelDto>(result);
    }

    public async Task<LabelDto> CreateAsync(LabelDto LabelDto)
    {
        Label result = await repository.CreateAsync(mapper.Map<Label>(LabelDto));

        await uowk.SaveAsync();

        return mapper.Map<LabelDto>(result);
    }
}
