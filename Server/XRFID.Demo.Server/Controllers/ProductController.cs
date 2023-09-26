using Microsoft.AspNetCore.Mvc;
using System.Data;
using Xerum.XFramework.Common;
using XRFID.Demo.Common.Dto;
using XRFID.Demo.Server.Services;

namespace XRFID.Demo.Server.Controllers;

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
        XResponseData response;
        try
        {
            response = _responseDataFactory.Ok<List<ProductDto>>(await _productService.GetAsync(term));
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

    [HttpGet("ByCode")]
    [ProducesResponseType(typeof(XResponseData<ProductDto>), 200)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 404)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> GetByCodeAsync(string code)
    {
        XResponseData response;
        try
        {
            response = _responseDataFactory.Ok<ProductDto?>(await _productService.GetByCodeAsync(code));
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

    [HttpGet("ByEpc")]
    [ProducesResponseType(typeof(XResponseData<ProductDto>), 200)]
    [ProducesResponseType(typeof(XResponseData), 400)]
    [ProducesResponseType(typeof(XResponseData), 404)]
    [ProducesResponseType(typeof(XResponseData), 500)]
    [ProducesResponseType(typeof(XResponseData), 501)]
    public async Task<IActionResult> GetByEpcAsync(string epc)
    {
        XResponseData response;
        try
        {
            response = _responseDataFactory.Ok<ProductDto?>(await _productService.GetByEpcAsync(epc));
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
        catch (Exception ex) when (ex is ArgumentException or DuplicateNameException)
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
