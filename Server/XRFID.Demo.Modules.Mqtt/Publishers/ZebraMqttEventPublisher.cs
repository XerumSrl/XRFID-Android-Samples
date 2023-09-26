using MassTransit;
using Microsoft.Extensions.Logging;
using XRFID.Demo.Modules.Mqtt.Contracts;
using XRFID.Demo.Modules.Mqtt.Events;

namespace XRFID.Demo.Modules.Mqtt.Publishers;

/// <summary>
/// Utilizzata per pubblicare messaggi ai client
/// </summary>
public class ZebraMqttEventPublisher : IZebraMqttEventPublisher
{
    private readonly IBusControl busControl;
    private readonly ILogger<ZebraMqttEventPublisher> logger;
    public ZebraMqttEventPublisher(IBusControl busControl, ILogger<ZebraMqttEventPublisher> logger)
    {
        this.busControl = busControl;
        this.logger = logger;
    }

    public async Task Publish(Heartbeat request)
    {
        // consumed by public class HeartbeatConsumer : IRequestConsumer<Heartbeat>
        await busControl.Publish<Heartbeat>(request, ctx => { ctx.TimeToLive = TimeSpan.FromMinutes(5); });
    }

    public async Task Publish(ZebraGpiData request)
    {
        // consumed by public class ZebraGpioDataConsumer : IRequestConsumer<ZebraGpiData>
        await busControl.Publish<ZebraGpiData>(request, ctx => { ctx.TimeToLive = TimeSpan.FromMinutes(5); });
    }

    public async Task Publish(ZebraTagData request)
    {
        // consumed by ZebraTagDataConsumer : IRequestConsumer<ZebraTagData>
        await busControl.Publish<ZebraTagData>(request, ctx => { ctx.TimeToLive = TimeSpan.FromMinutes(5); });
    }
}
