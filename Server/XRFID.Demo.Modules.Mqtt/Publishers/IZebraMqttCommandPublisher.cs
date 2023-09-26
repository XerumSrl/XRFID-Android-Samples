using Xerum.XFramework.MassTransit;
using XRFID.Demo.Modules.Mqtt.Payloads;

namespace XRFID.Demo.Modules.Mqtt.Publishers;

public interface IZebraMqttCommandPublisher : IRequestPublisher<RAWMQTTCommands>
{
}
