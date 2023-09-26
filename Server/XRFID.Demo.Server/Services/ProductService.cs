using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using XRFID.Demo.Common.Dto;
using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;
using XRFID.Demo.Server.Repositories;

namespace XRFID.Demo.Server.Services;

public class ProductService
{
    private readonly ProductRepository repository;
    private readonly IMapper mapper;
    private readonly UnitOfWork uowk;

    public ProductService(ProductRepository repository, IMapper mapper, UnitOfWork uowk)
    {
        this.repository = repository;
        this.mapper = mapper;
        this.uowk = uowk;
    }

    public async Task<List<ProductDto>> GetAsync(string term)
    {
        List<Product> result = await repository.GetAsync(q => (q.Name != null && q.Name.Contains(term)) || (q.Code != null && q.Code.Contains(term)));
        if (result.IsNullOrEmpty())
        {
            throw new KeyNotFoundException("Resource not found");
        }
        return mapper.Map<List<ProductDto>>(result);
    }

    public async Task<ProductDto> GetByEpcAsync(string epc)
    {
        List<Product> resultList = await repository.GetAsync(q => q.Epc == epc);
        if (resultList.IsNullOrEmpty())
        {
            throw new KeyNotFoundException("Resource not found");
        }

        Product? result = resultList.FirstOrDefault() ?? throw new KeyNotFoundException("Resource not found");
        return mapper.Map<ProductDto>(result);
    }

    public async Task<ProductDto> CreateAsync(ProductDto loadingUnitDto)
    {
        Product result = await repository.CreateAsync(mapper.Map<Product>(loadingUnitDto));

        await uowk.SaveAsync();

        return mapper.Map<ProductDto>(result);
    }

    public async Task<ProductDto?> GetByCodeAsync(string code)
    {
        List<Product> resultList = await repository.GetAsync(q => q.Code == code);
        if (!resultList.Any())
        {
            throw new KeyNotFoundException("Resource not found");
        }

        Product? result = resultList.FirstOrDefault() ?? throw new KeyNotFoundException("Resource not found");
        return mapper.Map<ProductDto>(result);
    }
}
