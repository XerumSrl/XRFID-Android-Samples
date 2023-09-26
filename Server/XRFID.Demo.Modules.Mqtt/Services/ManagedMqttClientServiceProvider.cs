using XRFID.Demo.Modules.Mqtt.Interfaces;

namespace XRFID.Demo.Modules.Services;

public class ManagedMqttClientServiceProvider
{
    public readonly IManagedMqttClientService ManagedMqttClientService;

    public ManagedMqttClientServiceProvider(IManagedMqttClientService managedMqttClientService)
    {
        ManagedMqttClientService = managedMqttClientService;
    }
}
