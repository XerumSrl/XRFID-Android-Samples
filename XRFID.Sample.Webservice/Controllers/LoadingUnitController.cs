using Microsoft.AspNetCore.Mvc;
using Xerum.XFramework.Common;
using XRFID.Sample.Common.Dto;
using XRFID.Sample.Webservice.Services;

namespace XRFID.Sample.Webservice.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public class LoadingUnitController : ControllerBase
{
    private readonly XResponseDataFactory _responseDataFactory;
    private readonly ILogger<LoadingUnitController> _logger;
    private readonly LoadingUnitService _loadingUnitService;

    public LoadingUnitController(XResponseDataFactory responseDataFactory, ILogger<LoadingUnitController> logger, LoadingUnitService loadingUnitService)
    {
        _responseDataFactory = responseDataFactory;
        _logger = logger;
        _loadingUnitService = loadingUnitService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(XResponseData<List<LoadingUnitDto>>), 200)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 404)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> GetAsync()
    {
        XResponseData errorResponse;
        XResponseData<List<LoadingUnitDto>> response;
        try
        {
            response = _responseDataFactory.Ok<List<LoadingUnitDto>>(await _loadingUnitService.GetAsync());
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            errorResponse = _responseDataFactory.NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            errorResponse = _responseDataFactory.BadRequest(ex.Message);
        }
        catch (NotImplementedException ex)
        {
            errorResponse = _responseDataFactory.NotImplemented(ex.Message);
        }
        catch (Exception ex)
        {
            errorResponse = _responseDataFactory.InternalError(ex.Message);
        }

        return StatusCode(errorResponse.Code, errorResponse);
    }

    [HttpGet("ByReference")]
    [ProducesResponseType(typeof(XResponseData<LoadingUnitDto>), 200)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 404)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> GetByReference(string reference)
    {
        XResponseData errorResponse;
        XResponseData<LoadingUnitDto> response;
        try
        {
            response = _responseDataFactory.Ok<LoadingUnitDto>(await _loadingUnitService.GetByReferenceAsync(reference));
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            errorResponse = _responseDataFactory.NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            errorResponse = _responseDataFactory.BadRequest(ex.Message);
        }
        catch (NotImplementedException ex)
        {
            errorResponse = _responseDataFactory.NotImplemented(ex.Message);
        }
        catch (Exception ex)
        {
            errorResponse = _responseDataFactory.InternalError(ex.Message);
        }

        return StatusCode(errorResponse.Code, errorResponse);
    }

    [HttpGet("WithItems")]
    [ProducesResponseType(typeof(XResponseData<LoadingUnitDto>), 200)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 404)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> GetWithItems(string reference)
    {
        XResponseData errorResponse;
        XResponseData<LoadingUnitDto> response;
        try
        {
            response = _responseDataFactory.Ok<LoadingUnitDto>(await _loadingUnitService.GetWithItemsAsync(reference));
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            errorResponse = _responseDataFactory.NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            errorResponse = _responseDataFactory.BadRequest(ex.Message);
        }
        catch (NotImplementedException ex)
        {
            errorResponse = _responseDataFactory.NotImplemented(ex.Message);
        }
        catch (Exception ex)
        {
            errorResponse = _responseDataFactory.InternalError(ex.Message);
        }

        return StatusCode(errorResponse.Code, errorResponse);
    }

    [HttpPost]
    [ProducesResponseType(typeof(XResponseData<LoadingUnitDto>), 201)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> PostAsync(LoadingUnitDto movementCreateDto)
    {
        XResponseData response;
        try
        {
            response = _responseDataFactory.Created<LoadingUnitDto>(await _loadingUnitService.CreateAsync(movementCreateDto));
        }
        catch (KeyNotFoundException ex)
        {
            response = _responseDataFactory.NotFound(movementCreateDto, ex.Message);
        }
        catch (ArgumentException ex)
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
