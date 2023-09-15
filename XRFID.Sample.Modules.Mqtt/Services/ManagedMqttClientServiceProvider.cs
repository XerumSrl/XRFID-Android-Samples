using XRFID.Sample.Modules.Mqtt.Interfaces;

namespace XRFID.Sample.Modules.Services;

public class ManagedMqttClientServiceProvider
{
    public readonly IManagedMqttClientService ManagedMqttClientService;

    public ManagedMqttClientServiceProvider(IManagedMqttClientService managedMqttClientService)
    {
        ManagedMqttClientService = managedMqttClientService;
    }
}
