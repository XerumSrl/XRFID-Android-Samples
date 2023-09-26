using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using XRFID.Demo.Modules.Mqtt.Interfaces;
using XRFID.Demo.Modules.Mqtt.Publishers;
using XRFID.Demo.Modules.Mqtt.Services;

namespace XRFID.Demo.Modules.Mqtt;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// Managed MQTT Client
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddZebraManagedMqttClient(this IServiceCollection services, Action<ManagedMqttClientConfiguration> configure)
    {
        var options = new ManagedMqttClientConfiguration();
        configure(options);

        //managed client
        services.AddSingleton<ManagedMqttClientOptions>(serviceProvider =>
        {
            var optionBuilder = new ManagedMqttClientOptionsBuilder();
            optionBuilder
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(options.ClientId.ToString())
                    .WithTcpServer(options.Server, options.Port)
                    .WithCredentials(options.Username, options.Password)
                    .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
                    .Build())
                .Build();
            return optionBuilder.Build();
        });
        services.AddSingleton<MqttMessageEventListener>();
        services.AddSingleton<IManagedMqttClientService, ManagedMqttClientService>();
        services.AddSingleton<IHostedService>(serviceProvider =>
        {
            return serviceProvider.GetService<IManagedMqttClientService>();
        });

        services.AddTransient<IZebraMqttEventPublisher, ZebraMqttEventPublisher>();
        services.AddTransient<IZebraMqttPayloadPublisher, ZebraMqttPayloadPublisher>();

        services.AddAutoMapper(cfg =>
        {
            //profiles
            cfg.AddProfile<MqttAutomapperProfile>();

        });

        return services;
    }
}

public class ManagedMqttClientConfiguration
{
    public string ClientId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Server { get; set; }
    private int? _port = 1883;
    public int? Port { get => _port; set => _port = value; }

}
