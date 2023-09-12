using Xerum.XFramework.MassTransit;
using XRFID.Sample.Modules.Mqtt.Contracts;
using XRFID.Sample.Modules.Mqtt.Events;

namespace XRFID.Sample.Modules.Mqtt.Publishers;

public interface IZebraMqttEventPublisher : IRequestPublisher<Heartbeat>, IRequestPublisher<ZebraGpiData>, IRequestPublisher<ZebraTagData>
{
}
