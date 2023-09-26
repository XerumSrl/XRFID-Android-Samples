using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Xerum.XFramework.Common;
using XRFID.Demo.Common.Dto;
using XRFID.Demo.Server.Services;

namespace XRFID.Demo.Server.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public class PrintController : ControllerBase
{
    private readonly XResponseDataFactory _responseDataFactory;
    private readonly ILogger<PrintController> _logger;
    private readonly PrinterService printerService;
    private readonly LabelService labelService;

    public PrintController(XResponseDataFactory responseDataFactory, ILogger<PrintController> logger,
        PrinterService printerService,
        LabelService labelService)
    {
        _responseDataFactory = responseDataFactory;
        _logger = logger;
        this.printerService = printerService;
        this.labelService = labelService;
    }

    [HttpPost]
    public async Task<ResponseData> Post([FromBody] PrintLabelDto printLabel)
    {
        if (printLabel == null)
        {
            var response = new ResponseData
            {
                Status = "Error",
                Code = "801",
                Message = "Label Input Error",
            };

            _logger.LogWarning("PrintController|{Status} Code: {Code}", response.Status, response.Code);

            return response;
        }
        var printer = await printerService.GetByIdAsync(printLabel.PrinterId);

        if (printer is null)
        {
            printer = await printerService.GetByNameAsync(printLabel.PrinterName);
        }

        if (printer == null)
        {
            var response = new ResponseData
            {
                Status = "Error",
                Code = "802",
                Message = "Get Printer Error",
            };

            _logger.LogWarning("PrintController|{Status} Code: {Code}", response.Status, response.Code);

            return response;
        }

        var label = await labelService.GetByIdAsync(printLabel.LabelId);
        if (label is null)
        {
            label = await labelService.GetByNameAsync(printLabel.LabelName);
        }

        if (label == null)
        {
            var response = new ResponseData
            {
                Status = "Error",
                Code = "803",
                Message = "Get Template Error",
            };

            _logger.LogWarning("PrintController|{Status} Code: {Code}", response.Status, response.Code);

            return response;
        }

        try
        {
            if (!PingHost(printer.Ip))
            {
                throw new Exception($"Printer {printer.Ip} not reachable");
            }
            else
            {
                _logger.LogDebug("Ping {Ip} Success", printer.Ip);
            }

            if (printer.Language != label.Language)
            {
                throw new Exception($"Label language: {label.Language} and Printer language: {printer.Language} MISMATCH");
            }
            else
            {
                _logger.LogDebug("Valid label language: {Language}", label.Language);
            }

            using (TcpClient client = new TcpClient(printer.Ip, printer.Port))
            {
                NetworkStream networkStream = client.GetStream();
                StreamWriter writer = new StreamWriter(networkStream);

                writer.AutoFlush = false;

                for (int curLabel = 1; curLabel <= printLabel.LabelQuantity; curLabel++)
                {
                    var labelContent = label.Content;
                    foreach (var variable in printLabel.Variables)
                    {
                        string oldValue = "{" + variable.Key + "}";
                        string newValue = variable.Value;
                        labelContent = labelContent.Replace(oldValue, newValue);
                    }

                    // progressivo numerazione
                    string currentLabel = string.Format($"{curLabel}/{printLabel.LabelQuantity}");
                    labelContent = labelContent.Replace("{currentLabel}", currentLabel);

                    _logger.LogTrace("[Post] printed label: \n{labelContent}", labelContent);

                    labelContent += "\r\n";

                    await writer.WriteAsync(labelContent);
                    await writer.FlushAsync();
                }
            }

            var response = new ResponseData
            {
                Status = "OK",
                Code = "899",
                Message = "Print Successful",
            };

            _logger.LogInformation("PrintController|{Status} Code: {Code}", response.Status, response.Code);

            return response;
        }
        catch (Exception ex)
        {
            var response = new ResponseData
            {
                Status = "Error",
                Code = "800",
                Message = ex.Message,
            };

            _logger.LogWarning("PrintController|{Status} Code: {Code}", response.Status, response.Code);
            _logger.LogError("PrintController|{Message}", ex.Message);

            return response;
        }

    }


    private bool PingHost(string nameOrAddress)
    {
        bool pingable = false;
        Ping pinger = null;

        try
        {
            pinger = new Ping();
            PingReply reply = pinger.Send(nameOrAddress);
            pingable = reply.Status == IPStatus.Success;
        }
        catch (PingException)
        {
            // Discard PingExceptions and return false;
        }
        finally
        {
            if (pinger != null)
            {
                pinger.Dispose();
            }
        }

        return pingable;
    }

}
