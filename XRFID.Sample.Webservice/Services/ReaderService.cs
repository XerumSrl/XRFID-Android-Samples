using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using XRFID.Sample.Common.Dto;
using XRFID.Sample.Webservice.Database;
using XRFID.Sample.Webservice.Entities;
using XRFID.Sample.Webservice.Repositories;

namespace XRFID.Sample.Webservice.Services;

public class ReaderService
{
    private readonly ReaderRepository repository;
    private readonly IMapper mapper;
    private readonly UnitOfWork uowk;

    public ReaderService(ReaderRepository repository, IMapper mapper, UnitOfWork uowk)
    {
        this.repository = repository;
        this.mapper = mapper;
        this.uowk = uowk;
    }

    public async Task<ReaderDto> GetByNameAsync(string name)
    {
        List<Reader> resultList = await repository.GetAsync(q => q.Name == name);
        if (resultList.IsNullOrEmpty())
        {
            throw new KeyNotFoundException();
        }

        Reader? result = resultList.FirstOrDefault() ?? throw new KeyNotFoundException();
        return mapper.Map<ReaderDto>(result);
    }

    public async Task<ReaderDto> CreateAsync(ReaderDto loadingUnitDto)
    {
        Reader result = await repository.CreateAsync(mapper.Map<Reader>(loadingUnitDto));

        await uowk.SaveAsync();

        return mapper.Map<ReaderDto>(result);
    }
}
