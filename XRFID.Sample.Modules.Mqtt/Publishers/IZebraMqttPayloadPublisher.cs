using Xerum.XFramework.MassTransit;
using XRFID.Sample.Modules.Mqtt.Payloads;

namespace XRFID.Sample.Modules.Mqtt.Publishers;

public interface IZebraMqttPayloadPublisher : IRequestPublisher<GetVersionResponse>,
    IRequestPublisher<GetNetworkResponse>
{
}
