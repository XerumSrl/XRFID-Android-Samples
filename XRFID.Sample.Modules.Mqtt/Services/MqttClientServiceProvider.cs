using XRFID.Sample.Modules.Mqtt.Interfaces;

namespace XRFID.Sample.Modules.Mqtt.Services;

public class MqttClientServiceProvider
{
    public readonly IMqttClientService MqttClientService;

    public MqttClientServiceProvider(IMqttClientService mqttClientService)
    {
        MqttClientService = mqttClientService;
    }
}
