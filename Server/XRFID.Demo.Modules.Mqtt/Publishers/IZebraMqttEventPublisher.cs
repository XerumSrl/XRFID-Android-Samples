using Xerum.XFramework.MassTransit;
using XRFID.Demo.Modules.Mqtt.Contracts;
using XRFID.Demo.Modules.Mqtt.Events;

namespace XRFID.Demo.Modules.Mqtt.Publishers;

public interface IZebraMqttEventPublisher : IRequestPublisher<Heartbeat>, IRequestPublisher<ZebraGpiData>, IRequestPublisher<ZebraTagData>
{
}
