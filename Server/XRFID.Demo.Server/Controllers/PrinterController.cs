using Microsoft.AspNetCore.Mvc;
using System.Data;
using Xerum.XFramework.Common;
using XRFID.Demo.Common.Dto;
using XRFID.Demo.Server.Services;

namespace XRFID.Demo.Server.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public class PrinterController : ControllerBase
{
    private readonly XResponseDataFactory _responseDataFactory;
    private readonly ILogger<PrinterController> _logger;
    private readonly PrinterService _printerService;

    public PrinterController(XResponseDataFactory responseDataFactory, ILogger<PrinterController> logger, PrinterService printerService)
    {
        _responseDataFactory = responseDataFactory;
        _logger = logger;
        _printerService = printerService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(XResponseData<List<PrinterDto>>), 200)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 404)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> GetBynameAsync()
    {
        XResponseData response;
        try
        {
            response = _responseDataFactory.Ok<List<PrinterDto>>(await _printerService.GetAsync());
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

    [HttpGet("ByName")]
    [ProducesResponseType(typeof(XResponseData<PrinterDto>), 200)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 404)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> GetBynameAsync(string name)
    {
        XResponseData response;
        try
        {
            response = _responseDataFactory.Ok<PrinterDto>(await _printerService.GetByNameAsync(name));
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
    [ProducesResponseType(typeof(XResponseData<PrinterDto>), 201)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> PostAsync(PrinterDto printerCreateDto)
    {
        XResponseData response;
        try
        {
            response = _responseDataFactory.Created<PrinterDto>(await _printerService.CreateAsync(printerCreateDto));
        }
        catch (Exception ex) when (ex is ArgumentException or DuplicateNameException)
        {
            response = _responseDataFactory.BadRequest(printerCreateDto, ex.Message);
        }
        catch (NotImplementedException ex)
        {
            response = _responseDataFactory.NotImplemented(printerCreateDto, ex.Message);
        }
        catch (Exception ex)
        {
            response = _responseDataFactory.InternalError(printerCreateDto, ex.Message);
        }

        return StatusCode(response.Code, response);
    }

}
