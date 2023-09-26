using XRFID.Demo.Modules.Mqtt.Interfaces;

namespace XRFID.Demo.Modules.Mqtt.Services;

public class MqttClientServiceProvider
{
    public readonly IMqttClientService MqttClientService;

    public MqttClientServiceProvider(IMqttClientService mqttClientService)
    {
        MqttClientService = mqttClientService;
    }
}
