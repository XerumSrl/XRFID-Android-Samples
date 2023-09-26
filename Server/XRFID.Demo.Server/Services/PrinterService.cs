using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using XRFID.Demo.Common.Dto;
using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;
using XRFID.Demo.Server.Repositories;

namespace XRFID.Demo.Server.Services;

public class PrinterService
{
    private readonly PrinterRepository repository;
    private readonly IMapper mapper;
    private readonly UnitOfWork uowk;

    public PrinterService(PrinterRepository repository, IMapper mapper, UnitOfWork uowk)
    {
        this.repository = repository;
        this.mapper = mapper;
        this.uowk = uowk;
    }

    public async Task<List<PrinterDto>> GetAsync()
    {
        List<Printer> result = await repository.GetAsync();

        if (!result.Any())
        {
            throw new KeyNotFoundException("Resource not found");
        }

        return mapper.Map<List<PrinterDto>>(result);
    }

    public async Task<PrinterDto> GetByNameAsync(string name)
    {
        List<Printer> resultList = await repository.GetAsync(q => q.Name == name);
        if (resultList.IsNullOrEmpty())
        {
            throw new KeyNotFoundException("Resource not found");
        }

        Printer? result = resultList.FirstOrDefault() ?? throw new KeyNotFoundException("Resource not found");
        return mapper.Map<PrinterDto>(result);
    }

    public async Task<PrinterDto> GetByIdAsync(Guid id)
    {
        List<Printer> resultList = await repository.GetAsync(q => q.Id == id);
        if (resultList.IsNullOrEmpty())
        {
            throw new KeyNotFoundException("Resource not found");
        }

        Printer? result = resultList.FirstOrDefault() ?? throw new KeyNotFoundException("Resource not found");
        return mapper.Map<PrinterDto>(result);
    }

    public async Task<PrinterDto> CreateAsync(PrinterDto PrinterDto)
    {
        Printer result = await repository.CreateAsync(mapper.Map<Printer>(PrinterDto));

        await uowk.SaveAsync();

        return mapper.Map<PrinterDto>(result);
    }

}
