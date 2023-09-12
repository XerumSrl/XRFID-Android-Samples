using Microsoft.Extensions.Hosting;
using MQTTnet.Extensions.ManagedClient;

namespace XRFID.Sample.Modules.Mqtt.Interfaces;

public interface IManagedMqttClientService : IHostedService
{
    IManagedMqttClient MqttClient { get; set; }

    Task EnqueueAsync(ManagedMqttApplicationMessage applicationMessage);
}