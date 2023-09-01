using Microsoft.AspNetCore.Mvc;
using Xerum.XFramework.Common;
using XRFID.Sample.Common.Dto;
using XRFID.Sample.Webservice.Services;

namespace XRFID.Sample.Webservice.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly XResponseDataFactory _responseDataFactory;
    private readonly ILogger<ProductController> _logger;
    private readonly ProductService _productService;

    public ProductController(XResponseDataFactory responseDataFactory, ILogger<ProductController> logger, ProductService productService)
    {
        _responseDataFactory = responseDataFactory;
        _logger = logger;
        _productService = productService;
    }

    [HttpGet("Search")]
    [ProducesResponseType(typeof(XResponseData<List<ProductDto>>), 200)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 404)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> GetSearchAsync(string term)
    {
        XResponseData errorResponse;
        XResponseData<List<ProductDto>> response;
        try
        {
            response = _responseDataFactory.Ok<List<ProductDto>>(await _productService.GetAsync(term));
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

    [HttpGet("ByEpc")]
    [ProducesResponseType(typeof(XResponseData<ProductDto>), 200)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 404)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> GetByEpcAsync(string epc)
    {
        XResponseData errorResponse;
        XResponseData<ProductDto?> response;
        try
        {
            response = _responseDataFactory.Ok<ProductDto?>(await _productService.GetByEpcAsync(epc));
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
    [ProducesResponseType(typeof(XResponseData<ProductDto>), 201)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> PostAsync(ProductDto productCreateDto)
    {
        XResponseData response;
        try
        {
            response = _responseDataFactory.Created<ProductDto>(await _productService.CreateAsync(productCreateDto));
        }
        catch (KeyNotFoundException ex)
        {
            response = _responseDataFactory.NotFound(productCreateDto, ex.Message);
        }
        catch (ArgumentException ex)
        {
            response = _responseDataFactory.BadRequest(productCreateDto, ex.Message);
        }
        catch (NotImplementedException ex)
        {
            response = _responseDataFactory.NotImplemented(productCreateDto, ex.Message);
        }
        catch (Exception ex)
        {
            response = _responseDataFactory.InternalError(productCreateDto, ex.Message);
        }

        return StatusCode(response.Code, response);
    }
}
