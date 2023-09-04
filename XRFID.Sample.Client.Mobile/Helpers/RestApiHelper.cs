﻿using RestSharp;
using System.Text.Json;
using Xerum.XFramework.Common;
using XRFID.Sample.Common.Dto;

namespace XRFID.Sample.Client.Mobile.Helpers;
#nullable enable

public class RestApiHelper
{
    private readonly string _baseApiUrl;
    private readonly RestClient _client;

    public RestApiHelper()
    {
        _baseApiUrl = Preferences.Default.Get<string>("api_endpoint", "https://localhost:5001/api/");

        //Client creation
        _client = new RestClient(_baseApiUrl);
    }

    #region Requests

    #region Reader
    public async Task<ReaderDto?> GetReaderByNameAsync(string name)
    {
        //Request
        var request = new RestRequest("Reader/ByName");
        request.AddQueryParameter("name", name);

        ReaderDto? result;

        try
        {
            RestResponse response = await _client.GetAsync(request);
            if ((int)response.StatusCode >= 400)
            {
                return null;
            }
            if (response.Content is null)
            {
                return null;
            }
            result = JsonSerializer.Deserialize<XResponseData<ReaderDto>>(response.Content)?.Result;
        }
        catch (Exception)
        {
            return null;
        }

        return result;
    }
    #endregion

    #region Product
    public async Task<List<ProductDto>?> GetProductsSearchAsync(string searchTerm)
    {
        //Request
        var request = new RestRequest("Product/Search");
        request.AddQueryParameter("term", searchTerm);

        List<ProductDto>? result;

        try
        {
            RestResponse response = await _client.GetAsync(request);
            if ((int)response.StatusCode >= 400)
            {
                return null;
            }
            if (response.Content is null)
            {
                return null;
            }
            result = JsonSerializer.Deserialize<XResponseData<List<ProductDto>>?>(response.Content)?.Result;
        }
        catch (Exception)
        {
            return null;
        }

        return result;
    }
    public async Task<ProductDto?> GetProductByEpcAsync(string epc)
    {
        //Request
        var request = new RestRequest("Product/ByEpc");
        request.AddQueryParameter("epc", epc);

        ProductDto? result;

        try
        {
            RestResponse response = await _client.GetAsync(request);
            if ((int)response.StatusCode >= 400)
            {
                return null;
            }
            if (response.Content is null)
            {
                return null;
            }
            result = JsonSerializer.Deserialize<XResponseData<ProductDto>>(response.Content)?.Result;
        }
        catch (Exception)
        {
            return null;
        }

        return result;
    }
    #endregion

    #region LoadingUnit
    public async Task<List<LoadingUnitDto>?> GetAllLoadingUnitAsync()
    {
        //Request
        var request = new RestRequest("LoadingUnit");
        XResponseData<List<LoadingUnitDto>>? result;

        try
        {
            RestResponse response = await _client.GetAsync(request);
            if ((int)response.StatusCode >= 400)
            {
                return null;
            }
            if (response.Content is null)
            {
                return null;
            }
            var content = response.Content;
            if (content is null)
            {
                return null;
            }
            result = JsonSerializer.Deserialize<XResponseData<List<LoadingUnitDto>>>(content);
            if (result is null)
            {
                return null;
            }
        }
        catch (Exception)
        {
            return null;
        }

        return result.Result;
    }
    public async Task<LoadingUnitDto?> GetLoadingUnitByRefAsync(string reference)
    {
        //Request
        var request = new RestRequest("LoadingUnit/ByReference");
        request.AddQueryParameter("reference", reference);

        LoadingUnitDto? result;

        try
        {
            RestResponse response = await _client.GetAsync(request);
            if ((int)response.StatusCode >= 400)
            {
                return null;
            }
            if (response.Content is null)
            {
                return null;
            }
            result = JsonSerializer.Deserialize<XResponseData<LoadingUnitDto>>(response.Content)?.Result;
        }
        catch (Exception)
        {
            return null;
        }

        return result;
    }
    public async Task<LoadingUnitDto?> GetLoadingUnitWithItemsAsync(string reference)
    {
        //Request
        var request = new RestRequest("LoadingUnit/WithItems");
        request.AddQueryParameter("reference", reference);

        LoadingUnitDto? result;

        try
        {
            RestResponse response = await _client.GetAsync(request);
            if ((int)response.StatusCode >= 400)
            {
                return null;
            }
            if (response.Content is null)
            {
                return null;
            }
            result = JsonSerializer.Deserialize<XResponseData<LoadingUnitDto>>(response.Content)?.Result;
        }
        catch (Exception)
        {
            return null;
        }

        return result;
    }
    public async Task<List<LoadingUnitItemDto>?> GetLoadingUnitItemsByLuIdAsync(Guid luId)
    {
        //Request
        var request = new RestRequest("LoadingUnitItem/ByLoadingUnitId");
        request.AddQueryParameter("luId", luId);

        List<LoadingUnitItemDto>? result;

        try
        {
            RestResponse response = await _client.GetAsync(request);
            if ((int)response.StatusCode >= 400)
            {
                return null;
            }
            if (response.Content is null)
            {
                return null;
            }
            result = JsonSerializer.Deserialize<XResponseData<List<LoadingUnitItemDto>>?>(response.Content)?.Result;
        }
        catch (Exception)
        {
            return null;
        }

        return result;
    }
    #endregion

    #region Movement
    public async Task<MovementDto?> CreateMovementAsync(MovementDto newMov)
    {
        //Request
        var request = new RestRequest("Movement");
        request.AddBody(newMov);

        MovementDto? result;

        try
        {
            RestResponse response = await _client.PostAsync(request);
            if ((int)response.StatusCode >= 400)
            {
                return null;
            }
            if (response.Content is null)
            {
                return null;
            }
            result = JsonSerializer.Deserialize<XResponseData<MovementDto>>(response.Content)?.Result;
        }
        catch (Exception ex)
        {
            return null;
        }

        return result;
    }
    #endregion

    #endregion
}
#nullable disable