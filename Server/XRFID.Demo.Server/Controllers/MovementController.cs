using Microsoft.AspNetCore.Mvc;
using System.Data;
using Xerum.XFramework.Common;
using XRFID.Demo.Common.Dto;
using XRFID.Demo.Server.Services;

namespace XRFID.Demo.Server.Controllers;
[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public class MovementController : ControllerBase
{
    private readonly XResponseDataFactory _responseDataFactory;
    private readonly ILogger<MovementController> _logger;
    private readonly MovementService _movementService;

    public MovementController(XResponseDataFactory responseDataFactory, ILogger<MovementController> logger, MovementService movementService)
    {
        _responseDataFactory = responseDataFactory;
        _logger = logger;
        _movementService = movementService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(XResponseData<MovementDto>), 201)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> PostAsync(MovementDto movementCreateDto)
    {
        XResponseData response;
        try
        {
            response = _responseDataFactory.Created<MovementDto>(await _movementService.CreateAsync(movementCreateDto));
        }
        catch (Exception ex) when (ex is ArgumentException or DuplicateNameException)
        {
            response = _responseDataFactory.BadRequest(movementCreateDto, ex.Message);
        }
        catch (NotImplementedException ex)
        {
            response = _responseDataFactory.NotImplemented(movementCreateDto, ex.Message);
        }
        catch (Exception ex)
        {
            response = _responseDataFactory.InternalError(movementCreateDto, ex.Message);
        }

        return StatusCode(response.Code, response);
    }
}
