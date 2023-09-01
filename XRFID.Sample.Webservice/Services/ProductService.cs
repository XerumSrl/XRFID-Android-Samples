using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using XRFID.Sample.Common.Dto;
using XRFID.Sample.Webservice.Database;
using XRFID.Sample.Webservice.Entities;
using XRFID.Sample.Webservice.Repositories;

namespace XRFID.Sample.Webservice.Services;

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
            throw new KeyNotFoundException();
        }
        return mapper.Map<List<ProductDto>>(result);
    }

    public async Task<ProductDto> GetByEpcAsync(string epc)
    {
        List<Product> resultList = await repository.GetAsync(q => q.Epc == epc);
        if (resultList.IsNullOrEmpty())
        {
            throw new KeyNotFoundException();
        }

        Product? result = resultList.FirstOrDefault() ?? throw new KeyNotFoundException();
        return mapper.Map<ProductDto>(result);
    }

    public async Task<ProductDto> CreateAsync(ProductDto loadingUnitDto)
    {
        Product result = await repository.CreateAsync(mapper.Map<Product>(loadingUnitDto));

        await uowk.SaveAsync();

        return mapper.Map<ProductDto>(result);
    }
}
