using AutoMapper;
using XRFID.Demo.Common.Dto;
using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;
using XRFID.Demo.Server.Repositories;

namespace XRFID.Demo.Server.Services;

public class MovementService
{
    private readonly MovementRepository repository;
    private readonly IMapper mapper;
    private readonly UnitOfWork uowk;

    public MovementService(MovementRepository repository, IMapper mapper, UnitOfWork uowk)
    {
        this.repository = repository;
        this.mapper = mapper;
        this.uowk = uowk;
    }

    public async Task<MovementDto> CreateAsync(MovementDto movementDto)
    {
        Movement result = await repository.CreateAsync(mapper.Map<Movement>(movementDto));

        await uowk.SaveAsync();

        return mapper.Map<MovementDto>(result);
    }
}
