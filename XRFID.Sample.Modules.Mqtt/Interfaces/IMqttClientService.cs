using Microsoft.Extensions.Hosting;
using MQTTnet;

namespace XRFID.Sample.Modules.Mqtt.Interfaces;

public interface IMqttClientService : IHostedService
{
    Task PublishAsync(MqttApplicationMessage applicationMessage);
}