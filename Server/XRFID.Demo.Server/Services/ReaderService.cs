using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using XRFID.Demo.Common.Dto;
using XRFID.Demo.Common.Dto.Create;
using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;
using XRFID.Demo.Server.Repositories;

namespace XRFID.Demo.Server.Services;

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
            throw new KeyNotFoundException("Resource not found");
        }

        Reader? result = resultList.FirstOrDefault() ?? throw new KeyNotFoundException("Resource not found");
        return mapper.Map<ReaderDto>(result);
    }

    public async Task<ReaderDto> CreateAsync(ReaderDto readerDto)
    {
        Reader result = await repository.CreateAsync(mapper.Map<Reader>(readerDto));

        await uowk.SaveAsync();

        return mapper.Map<ReaderDto>(result);
    }

    public async Task<ReaderDto> CreateAsync(MinimalReaderCreateDto readerDto)
    {
        Reader result = await repository.CreateAsync(new Reader { Id = readerDto.Id, Name = readerDto.Name });
        await uowk.SaveAsync();
        return mapper.Map<ReaderDto>(result);
    }
}
