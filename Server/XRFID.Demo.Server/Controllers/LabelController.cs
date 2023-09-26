using Microsoft.AspNetCore.Mvc;
using Xerum.XFramework.Common;
using XRFID.Demo.Common.Dto;
using XRFID.Demo.Server.Services;

namespace XRFID.Demo.Server.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public class LabelController : ControllerBase
{
    private readonly XResponseDataFactory _responseDataFactory;
    private readonly ILogger<LabelController> _logger;
    private readonly LabelService _printerService;

    public LabelController(XResponseDataFactory responseDataFactory, ILogger<LabelController> logger, LabelService printerService)
    {
        _responseDataFactory = responseDataFactory;
        _logger = logger;
        _printerService = printerService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(XResponseData<List<LabelDto>>), 200)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 404)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> GetAsync()
    {
        XResponseData response;
        try
        {
            response = _responseDataFactory.Ok<List<LabelDto>>(await _printerService.GetAsync());
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
}
