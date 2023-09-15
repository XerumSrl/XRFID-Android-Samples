using MassTransit;
using Microsoft.Extensions.Logging;
using XRFID.Sample.Modules.Mqtt.Payloads;

namespace XRFID.Sample.Modules.Mqtt.Publishers;

/// <summary>
/// Utilizzata per pubblicare messaggi ai client
/// </summary>
public class ZebraMqttCommandPublisher : IZebraMqttCommandPublisher
{
    private readonly IBusControl busControl;
    private readonly ILogger<ZebraMqttCommandPublisher> logger;
    public ZebraMqttCommandPublisher(IBusControl busControl, ILogger<ZebraMqttCommandPublisher> logger)
    {
        this.busControl = busControl;
        this.logger = logger;
    }

    //pubblica su ZebraMqtt.ZebraCommandConsumer
    public async Task Publish(RAWMQTTCommands request)
    {
        logger.LogTrace(request.ToString());
        await busControl.Publish(request);
    }
}
