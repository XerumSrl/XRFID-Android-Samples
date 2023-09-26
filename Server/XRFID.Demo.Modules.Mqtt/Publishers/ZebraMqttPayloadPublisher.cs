using MassTransit;
using XRFID.Demo.Modules.Mqtt.Payloads;

namespace XRFID.Demo.Modules.Mqtt.Publishers;

/// <summary>
/// Utilizzata per pubblicare messaggi ai client
/// </summary>
public class ZebraMqttPayloadPublisher : IZebraMqttPayloadPublisher
{
    private readonly IBusControl busControl;
    public ZebraMqttPayloadPublisher(IBusControl busControl)
    {
        this.busControl = busControl;
    }

    public async Task Publish(GetVersionResponse request)
    {
        await busControl.Publish<GetVersionResponse>(request);
    }

    public async Task Publish(GetNetworkResponse request)
    {
        await busControl.Publish<GetNetworkResponse>(request);
    }
}
