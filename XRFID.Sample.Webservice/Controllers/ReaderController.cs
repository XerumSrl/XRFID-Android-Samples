using Microsoft.AspNetCore.Mvc;
using Xerum.XFramework.Common;
using XRFID.Sample.Common.Dto;
using XRFID.Sample.Webservice.Services;

namespace XRFID.Sample.Webservice.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public class ReaderController : ControllerBase
{
    private readonly XResponseDataFactory _responseDataFactory;
    private readonly ILogger<ReaderController> _logger;
    private readonly ReaderService _readerService;

    public ReaderController(XResponseDataFactory responseDataFactory, ILogger<ReaderController> logger, ReaderService readerService)
    {
        _responseDataFactory = responseDataFactory;
        _logger = logger;
        _readerService = readerService;
    }

    [HttpGet("ByName")]
    [ProducesResponseType(typeof(XResponseData<ReaderDto>), 200)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 404)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> GetBynameAsync(string name)
    {
        XResponseData errorResponse;
        XResponseData<ReaderDto> response;
        try
        {
            response = _responseDataFactory.Ok<ReaderDto>(await _readerService.GetByNameAsync(name));
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
    [ProducesResponseType(typeof(XResponseData<ReaderDto>), 201)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> PostAsync(ReaderDto readerCreateDto)
    {
        XResponseData response;
        try
        {
            response = _responseDataFactory.Created<ReaderDto>(await _readerService.CreateAsync(readerCreateDto));
        }
        catch (KeyNotFoundException ex)
        {
            response = _responseDataFactory.NotFound(readerCreateDto, ex.Message);
        }
        catch (ArgumentException ex)
        {
            response = _responseDataFactory.BadRequest(readerCreateDto, ex.Message);
        }
        catch (NotImplementedException ex)
        {
            response = _responseDataFactory.NotImplemented(readerCreateDto, ex.Message);
        }
        catch (Exception ex)
        {
            response = _responseDataFactory.InternalError(readerCreateDto, ex.Message);
        }

        return StatusCode(response.Code, response);
    }
}
