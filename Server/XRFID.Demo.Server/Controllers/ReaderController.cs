using Microsoft.AspNetCore.Mvc;
using System.Data;
using Xerum.XFramework.Common;
using XRFID.Demo.Common.Dto;
using XRFID.Demo.Common.Dto.Create;
using XRFID.Demo.Server.Services;

namespace XRFID.Demo.Server.Controllers;

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
        XResponseData response;
        try
        {
            response = _responseDataFactory.Ok<ReaderDto>(await _readerService.GetByNameAsync(name));
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
        catch (Exception ex) when (ex is ArgumentException or DuplicateNameException)
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

    [HttpPost("Minimal")]
    [ProducesResponseType(typeof(XResponseData<ReaderDto>), 201)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> PostMinimalAsync(MinimalReaderCreateDto minimalReaderCreateDto)
    {
        XResponseData response;
        try
        {
            response = _responseDataFactory.Created<ReaderDto>(await _readerService.CreateAsync(minimalReaderCreateDto));
        }
        catch (Exception ex) when (ex is ArgumentException or DuplicateNameException)
        {
            response = _responseDataFactory.BadRequest<MinimalReaderCreateDto>(minimalReaderCreateDto, ex.Message);
        }
        catch (NotImplementedException ex)
        {
            response = _responseDataFactory.NotImplemented<MinimalReaderCreateDto>(minimalReaderCreateDto, ex.Message);
        }
        catch (Exception ex)
        {
            response = _responseDataFactory.InternalError<MinimalReaderCreateDto>(minimalReaderCreateDto, ex.Message);
        }

        return StatusCode(response.Code, response);
    }
}
