using Microsoft.AspNetCore.Mvc;
using System.Data;
using Xerum.XFramework.Common;
using XRFID.Demo.Common.Dto;
using XRFID.Demo.Server.Services;

namespace XRFID.Demo.Server.Controllers;

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
        XResponseData response;
        try
        {
            response = _responseDataFactory.Ok<List<LoadingUnitDto>>(await _loadingUnitService.GetAsync());
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

    [HttpGet("ByReference")]
    [ProducesResponseType(typeof(XResponseData<LoadingUnitDto>), 200)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 404)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> GetByReference(string reference)
    {
        XResponseData response;
        try
        {
            response = _responseDataFactory.Ok<LoadingUnitDto>(await _loadingUnitService.GetByReferenceAsync(reference));
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

    [HttpGet("WithItems")]
    [ProducesResponseType(typeof(XResponseData<LoadingUnitDto>), 200)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 404)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> GetWithItems(string reference)
    {
        XResponseData response;
        try
        {
            response = _responseDataFactory.Ok<LoadingUnitDto>(await _loadingUnitService.GetWithItemsAsync(reference));
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
