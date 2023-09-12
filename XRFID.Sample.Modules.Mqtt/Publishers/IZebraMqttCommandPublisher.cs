using Xerum.XFramework.MassTransit;
using XRFID.Sample.Modules.Mqtt.Payloads;

namespace XRFID.Sample.Modules.Mqtt.Publishers;

public interface IZebraMqttCommandPublisher : IRequestPublisher<RAWMQTTCommands>
{
}
