using Microsoft.AspNetCore.Mvc;
using System.Data;
using Xerum.XFramework.Common;
using XRFID.Demo.Common.Dto;
using XRFID.Demo.Server.Services;

namespace XRFID.Demo.Server.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public class LoadingUnitItemController : ControllerBase
{
    private readonly XResponseDataFactory _responseDataFactory;
    private readonly ILogger<LoadingUnitItemController> _logger;
    private readonly LoadingUnitItemService _loadingUnitItemService;

    public LoadingUnitItemController(XResponseDataFactory responseDataFactory, ILogger<LoadingUnitItemController> logger, LoadingUnitItemService loadingUnitItemService)
    {
        _responseDataFactory = responseDataFactory;
        _logger = logger;
        _loadingUnitItemService = loadingUnitItemService;
    }

    [HttpGet("ByLoadingUnitId")]
    [ProducesResponseType(typeof(XResponseData<List<LoadingUnitItemDto>>), 200)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 404)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> GetByLoadingUnitIdAsync(Guid luId)
    {
        XResponseData response;
        try
        {
            response = _responseDataFactory.Ok<List<LoadingUnitItemDto>>(await _loadingUnitItemService.GetByLoadingUnitIdAsync(luId));
        }
        catch (KeyNotFoundException ex)
        {
            response = _responseDataFactory.NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            response = _responseDataFactory.BadRequest(ex.Message);
        }
        catch (NotImplementedException ex)
        {
            response = _responseDataFactory.NotImplemented(ex.Message);
        }
        catch (Exception ex)
        {
            response = _responseDataFactory.InternalError(ex.Message);
        }

        return StatusCode(response.Code, response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(XResponseData<LoadingUnitItemDto>), 201)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> PostAsync(LoadingUnitItemDto loadingUnitItemCreateDto)
    {
        XResponseData response;
        try
        {
            response = _responseDataFactory.Created<LoadingUnitItemDto>(await _loadingUnitItemService.CreateAsync(loadingUnitItemCreateDto));
        }
        catch (Exception ex) when (ex is ArgumentException or DuplicateNameException)
        {
            response = _responseDataFactory.BadRequest(loadingUnitItemCreateDto, ex.Message);
        }
        catch (NotImplementedException ex)
        {
            response = _responseDataFactory.NotImplemented(loadingUnitItemCreateDto, ex.Message);
        }
        catch (Exception ex)
        {
            response = _responseDataFactory.InternalError(loadingUnitItemCreateDto, ex.Message);
        }

        return StatusCode(response.Code, response);
    }
}
