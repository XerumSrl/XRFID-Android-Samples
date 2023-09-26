using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using XRFID.Demo.Modules.Mqtt.Contracts;
using XRFID.Demo.Modules.Mqtt.Events;
using XRFID.Demo.Modules.Mqtt.Interfaces;
using XRFID.Demo.Modules.Mqtt.Payloads;

namespace XRFID.Demo.Modules.Mqtt.Services;


public class MqttMessageEventListener
{
    IServiceScopeFactory serviceScopeFactory;
    IServiceProvider serviceProvider;
    private readonly ILogger<MqttMessageEventListener> logger;

    public MqttMessageEventListener(IServiceProvider serviceProvider,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<MqttMessageEventListener> logger)
    {
        this.serviceProvider = serviceProvider;
        this.serviceScopeFactory = serviceScopeFactory;
        this.logger = logger;
        //this.mqttClient = mqttClient;
    }

    public void HandleEvent(Hello evt, string t)
    {
        logger.LogDebug(evt.ToString());
    }
    public async Task HandleEvent(Heartbeat evt, string t)
    {
        logger.LogDebug(evt.ToString());

    }

    public async Task HandleEvent(ZebraGpiData evt, string t)
    {
        //using (var scope = serviceScopeFactory.CreateScope())
        //{
        //    var gpInClient = scope.ServiceProvider.GetService<IRequestClient<SubmitGpIn>>();

        //    var status = await gpInClient.GetResponse<GpInStatus>(new {
        //        Id = evt.Pin,
        //        Value = evt.Value,
        //        Topic = t,
        //    });
        //}

        logger.LogDebug(evt.ToString());
    }

    //------------------------------------------------------------
    public async Task HandleEvent(ZebraTagData evt, string t)
    {
        logger.LogDebug(evt.ToString());
    }
    //-------------------------------------------------------------

    public void HandleEvent(FirmwareUpdateProgress evt, string t)
    {
        logger.LogDebug(evt.ToString());
    }
    public void HandleEvent(Error evt, string t)
    {
        logger.LogDebug(evt.ToString());
    }
    public void HandleEvent(Warning evt, string t)
    {
        logger.LogDebug(evt.ToString());
    }
    public void HandleEvent(Userapp evt, string t)
    {
        logger.LogDebug(evt.ToString());
    }

    public void Listen(IMqttMessageData evt, string t)
    {
        dynamic eve = evt;

        try
        {
            HandleEvent(eve, t);
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch
        {
            // ignore
        }
    }


    public void HandleEvent(GetVersionResponse evt, string t)
    {
        logger.LogDebug(evt.ToString());
    }
    public async void HandleEvent(GetNetworkResponse evt, string t)
    {
        //using (var scope = serviceScopeFactory.CreateScope())
        //{
        //    IRequestSender<InitConnection, InitConnectionResponse> initConnectionPublisher = scope.ServiceProvider.GetService<IRequestSender<InitConnection, InitConnectionResponse>>();

        //    logger.LogDebug($"Send InitConnection Uid:{evt.MacAddress}");
        //    InitConnectionResponse initConnectionResponse = await initConnectionPublisher.Send(new InitConnection()
        //    {
        //        ReaderName = evt.HostName,
        //        SerialNumber = evt.MacAddress,
        //        //ReaderIp = "localhost"
        //        ReaderIp = evt.IpAddress
        //    });
        //    logger.LogDebug(@"InitConnection Response: {initConnectionResponse}", initConnectionResponse);
        //    if (initConnectionResponse.LicenseStatus != LicenseStatus.VALID)
        //    {
        //        logger.LogError("Invalid license");
        //        return;
        //    }
        //    logger.LogDebug("InitConnection Completed");
        //}

        logger.LogDebug(evt.ToString());
    }
    public void HandleEvent(GetStatusResponse evt, string t)
    {
        logger.LogDebug(evt.ToString());
    }
    public void HandleEvent(GetModeResponse evt, string t)
    {
        logger.LogDebug(evt.ToString());
    }

    public void HandleEvent(SetGpoResponse evt, string t)
    {
        logger.LogDebug(evt.ToString());
    }

    public void Listen(IMqttResponseData evt, string t)
    {
        dynamic eve = evt;

        try
        {
            HandleEvent(eve, t);
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch
        {
            // ignore
        }
    }
}