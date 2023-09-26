using Xerum.XFramework.MassTransit;
using XRFID.Demo.Modules.Mqtt.Payloads;

namespace XRFID.Demo.Modules.Mqtt.Publishers;

public interface IZebraMqttPayloadPublisher : IRequestPublisher<GetVersionResponse>,
    IRequestPublisher<GetNetworkResponse>
{
}
