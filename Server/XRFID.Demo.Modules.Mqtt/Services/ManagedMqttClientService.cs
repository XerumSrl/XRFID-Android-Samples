using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using XRFID.Demo.Modules.Mqtt.Contracts;
using XRFID.Demo.Modules.Mqtt.Events;
using XRFID.Demo.Modules.Mqtt.Interfaces;
using XRFID.Demo.Modules.Mqtt.JsonConverters;
using XRFID.Demo.Modules.Mqtt.Payloads;
using XRFID.Demo.Modules.Mqtt.Publishers;

namespace XRFID.Demo.Modules.Mqtt.Services;

public class ManagedMqttClientService : IManagedMqttClientService
{
    private readonly IMapper mapper;
    private readonly ILogger<ManagedMqttClientService> logger;
    private ManagedMqttClientOptions options;
    private IServiceScopeFactory serviceScopeFactory;

    public IManagedMqttClient MqttClient { get; set; }

    public ManagedMqttClientService(ManagedMqttClientOptions options,
        IServiceScopeFactory serviceScopeFactory,
        IMapper mapper,
        ILogger<ManagedMqttClientService> logger)
    {
        this.mapper = mapper;
        this.logger = logger;
        this.options = options;
        this.serviceScopeFactory = serviceScopeFactory;

        MqttClient = new MqttFactory().CreateManagedMqttClient();
        ConfigureMqttClient();
    }

    private void ConfigureMqttClient()
    {
        logger.LogTrace("[ConfigureMqttClient] Configure Mqtt Client with Client Id {ClientId}", options.ClientOptions.ClientId);

        MqttClient.ConnectedAsync += async (arg) =>
        {
            //https://github.com/dotnet/MQTTnet/wiki/MQTT-topics
            logger.LogInformation("[ConfigureMqttClient] Connected with Client Id {ClientId}", options.ClientOptions.ClientId);

            //per convenzione xerum, i reader zebra sono configurati per pubblicare su
            // - topic / subtopic con subtopic uguale a nome univoco del reader (es. mcmds/FX9600FBDB58)
            await MqttClient.SubscribeAsync(new List<MqttTopicFilter>
            {
                new MqttTopicFilterBuilder()
                                .WithTopic("tevents/+")
                                .WithExactlyOnceQoS()
                                .Build(),
                new MqttTopicFilterBuilder()
                                .WithTopic("mevents/+")
                                .WithExactlyOnceQoS()
                                .Build(),
                new MqttTopicFilterBuilder()
                                .WithTopic("mcmds/+")
                                .WithExactlyOnceQoS()
                                .Build(),
                new MqttTopicFilterBuilder()
                                .WithTopic("mrsp/+")
                                .WithExactlyOnceQoS()
                                .Build(),
                new MqttTopicFilterBuilder()
                                .WithTopic("ccmds/+")
                                .WithExactlyOnceQoS()
                                .Build(),
                new MqttTopicFilterBuilder()
                                .WithTopic("crsp/+")
                                .WithExactlyOnceQoS()
                                .Build()

            });
        };

        MqttClient.DisconnectedAsync += async (arg) =>
        {
            logger.LogInformation("[ConfigureMqttClient] Disconnected");
            //await Task.Delay(TimeSpan.FromSeconds(5));

            //try
            //{
            //    await mqttClient.ConnectAsync(options);
            //}
            //catch
            //{
            //    Console.WriteLine("### RECONNECTING FAILED ###");
            //}

        };

        MqttClient.ApplicationMessageReceivedAsync += async (arg) =>
        {
            //logger.LogDebug("ManagedMqttClientService: received application message");
            //logger.LogDebug($"+ Topic = {arg.ApplicationMessage.Topic}");
            //logger.LogDebug($"+ Payload = {Encoding.UTF8.GetString(arg.ApplicationMessage.Payload)}");
            //logger.LogDebug($"+ QoS = {arg.ApplicationMessage.QualityOfServiceLevel}");
            //logger.LogDebug($"+ Retain = {arg.ApplicationMessage.Retain}");

            logger.LogDebug("[ConfigureMqttClient] Topic: {Topic}", arg.ApplicationMessage.Topic);
            logger.LogDebug("[ConfigureMqttClient] Payload: {Payload}", Encoding.UTF8.GetString(arg.ApplicationMessage.Payload));

            if (arg.ApplicationMessage.Topic.StartsWith("mevents/"))
            {
                try
                {


                    ZebraMqttApplicationMessage evt = mapper.Map<ZebraMqttApplicationMessage>(arg.ApplicationMessage);
                    JsonSerializerOptions options = new JsonSerializerOptions();

                    switch (evt.ZebraManagementEvents?.Type)
                    {
                        case "hello":
                            //listener.Listen(JsonConvert.DeserializeObject<Hello>($"{evt.ZebraManagementEvents.Data}", options), arg.ApplicationMessage.Topic);
                            break;
                        case "heartbeat":

                            var heartbeat = JsonSerializer.Deserialize<Heartbeat>($"{evt.ZebraManagementEvents.Data}", options);

                            //listener.Listen(heartbeat, arg.ApplicationMessage.Topic);

                            using (var scope = serviceScopeFactory.CreateScope())
                            {
                                //var client = scope.ServiceProvider.GetService<IRequestPublisher<Heartbeat>>();
                                var client = scope.ServiceProvider.GetService<IZebraMqttEventPublisher>();

                                string pattern = @"(/)";
                                Regex regex = new Regex(pattern);
                                var hostname = regex.Split(evt.Topic).Last().ToUpper();
                                heartbeat.HostName = hostname;

                                await client.Publish(heartbeat);

                            }

                            break;
                        case "gpi":
                            var gpi = JsonSerializer.Deserialize<ZebraGpiData>($"{evt.ZebraManagementEvents.Data}", options);
                            //listener.Listen(gpi, arg.ApplicationMessage.Topic);
                            logger.LogTrace("[Mevents] GPI: {gpi}", gpi);

                            using (var scope = serviceScopeFactory.CreateScope())
                            {
                                //var client = scope.ServiceProvider.GetService<IRequestPublisher<GpiEvent>>();
                                var client = scope.ServiceProvider.GetService<IZebraMqttEventPublisher>();

                                string pattern = @"(/)";
                                Regex regex = new Regex(pattern);
                                var hostname = regex.Split(evt.Topic).Last().ToUpper();
                                gpi.HostName = hostname;

                                await client.Publish(gpi);

                                //    // var client = scope.ServiceProvider.GetService<IRequestClient<SubmitGpIn>>();

                                //    //var status = await client.GetResponse<GpInStatus>(new {

                                //    //    ReaderId = Guid.Empty,
                                //    //    Id = gpi.Pin,
                                //    //    Value = gpi.Value,
                                //    //    Direction = "",
                                //    //    Topic = arg.ApplicationMessage.Topic
                                //    //});
                                //    //logger.LogDebug(JsonConvert.SerializeObject(status, Formatting.Indented));
                                //    // *****************************************************************
                            }

                            break;
                        case "firmwareUpdateProgress":
                            //listener.Listen(JsonConvert.DeserializeObject<FirmwareUpdateProgress>($"{evt.ZebraManagementEvents.Data}", options), arg.ApplicationMessage.Topic);
                            break;
                        case "error":
                            //listener.Listen(JsonConvert.DeserializeObject<Error>($"{evt.ZebraManagementEvents.Data}", options), arg.ApplicationMessage.Topic);
                            break;
                        case "warning":
                            //listener.Listen(JsonConvert.DeserializeObject<Warning>($"{evt.ZebraManagementEvents.Data}", options), arg.ApplicationMessage.Topic);
                            break;
                        case "userapp":
                            //listener.Listen(JsonConvert.DeserializeObject<Userapp>($"{evt.ZebraManagementEvents.Data}", options), arg.ApplicationMessage.Topic);
                            break;
                        default:
                            throw new Exception($"No event type defined for: {evt.ZebraManagementEvents.Type}");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "[ConfigureMqttClient] Unexpected Exception");
                }
            }

            //---------------------------------------------- TEVENTS ----------------------------------------------------------------

            if (arg.ApplicationMessage.Topic.StartsWith("tevents/"))
            {
                try
                {
                    ZebraMqttApplicationMessage evt = mapper.Map<ZebraMqttApplicationMessage>(arg.ApplicationMessage);
                    var options = new JsonSerializerOptions();
                    options.Converters.Add(new DateTimeConverter());

                    string pattern = @"(/)";
                    Regex regex = new Regex(pattern);
                    var hostname = regex.Split(evt.Topic).Last().ToUpper();

                    switch (evt.ZebraTagEvent.Type.ToUpper())
                    {
                        case "SIMPLE":
                            //da tagevent.data diventa tagdata
                            var tagSimple = JsonSerializer.Deserialize<ZebraTagData>($"{evt.ZebraTagEvent.Data}", options);

                            logger.LogTrace("[Tevents] evt.ZebraTagEvent SIMPLE: {tagSimple}", tagSimple);

                            using (var scope = serviceScopeFactory.CreateScope())
                            {
                                var client = scope.ServiceProvider.GetService<IZebraMqttEventPublisher>();

                                tagSimple.HostName = hostname;
                                await client.Publish(tagSimple);

                            }
                            break;

                        case "PORTAL":
                            //da tagevent.data diventa tagdata
                            var tagPortal = JsonSerializer.Deserialize<ZebraTagData>($"{evt.ZebraTagEvent.Data}", options);

                            logger.LogTrace("[Tevents] evt.ZebraTagEvent PORTAL: {tagPortal}", tagPortal);

                            using (var scope = serviceScopeFactory.CreateScope())
                            {
                                var client = scope.ServiceProvider.GetService<IZebraMqttEventPublisher>();

                                tagPortal.HostName = hostname;
                                await client.Publish(tagPortal);

                            }
                            break;

                        case "CONVEYOR":
                            //da tagevent.data diventa tagdata
                            var tagConveyor = JsonSerializer.Deserialize<ZebraTagData>($"{evt.ZebraTagEvent.Data}", options);

                            logger.LogTrace("[Tevents] evt.ZebraTagEvent CONVEYOR: {tagConveyor}", tagConveyor);

                            using (var scope = serviceScopeFactory.CreateScope())
                            {
                                var client = scope.ServiceProvider.GetService<IZebraMqttEventPublisher>();

                                tagConveyor.HostName = hostname;
                                await client.Publish(tagConveyor);

                            }
                            break;

                        case "CUSTOM":
                            //da tagevent.data diventa tagdata
                            var tagCustom = JsonSerializer.Deserialize<ZebraTagData>($"{evt.ZebraTagEvent.Data}", options);

                            logger.LogTrace("[Tevents] evt.ZebraTagEvent PORTAL: {tagCustom}", tagCustom);

                            using (var scope = serviceScopeFactory.CreateScope())
                            {
                                var client = scope.ServiceProvider.GetService<IZebraMqttEventPublisher>();

                                tagCustom.HostName = hostname;
                                await client.Publish(tagCustom);

                            }
                            break;

                        default:
                            throw new Exception($"ZebraTagEvent.Type not mapped: {evt.ZebraTagEvent.Type.ToUpper()}");

                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "[ConfigureMqttClient] Unexpected Exception");
                }
            }

            //--------------------------------------------- /TEVENTS ----------------------------------------------------------------

            if (arg.ApplicationMessage.Topic.StartsWith("mrsp/"))
            {
                try
                {
                    ZebraMqttApplicationMessage evt = mapper.Map<ZebraMqttApplicationMessage>(arg.ApplicationMessage);
                    var options = new JsonSerializerOptions();

                    string pattern = @"(/)";
                    Regex regex = new Regex(pattern);
                    var hostname = regex.Split(evt.Topic).Last().ToUpper();

                    switch (evt.ZebraMQTTResponses.Command)
                    {
                        case "get_version":
                            var version = JsonSerializer.Deserialize<GetVersionResponse>($"{evt.ZebraMQTTResponses.Payload}", options);

                            //listener.Listen(version, arg.ApplicationMessage.Topic);

                            using (var scope = serviceScopeFactory.CreateScope())
                            {
                                var client = scope.ServiceProvider.GetService<IZebraMqttPayloadPublisher>();

                                version.HostName = hostname;
                                await client.Publish(version);

                            }

                            break;

                        case "get_network":
                            var network = JsonSerializer.Deserialize<GetNetworkResponse>($"{evt.ZebraMQTTResponses.Payload}", options);

                            //listener.Listen(network, arg.ApplicationMessage.Topic);

                            using (var scope = serviceScopeFactory.CreateScope())
                            {
                                var client = scope.ServiceProvider.GetService<IZebraMqttPayloadPublisher>();

                                network.HostName = hostname;
                                await client.Publish(network);

                            }

                            break;
                        case "get_status":
                            //var status = JsonConvert.DeserializeObject<GetStatusResponse>($"{evt.ZebraMQTTResponses.Payload}", options);
                            //listener.Listen(status, arg.ApplicationMessage.Topic);
                            break;
                        case "get_mode":
                            //var mode = JsonConvert.DeserializeObject<GetModeResponse>($"{evt.ZebraMQTTResponses.Payload}", options);
                            //listener.Listen(mode, arg.ApplicationMessage.Topic);
                            break;
                        case "set_gpo":
                            //var gpo = JsonConvert.DeserializeObject<SetGpoResponse>($"{evt.ZebraMQTTResponses.Payload}", options);
                            //listener.Listen(gpo, arg.ApplicationMessage.Topic);
                            break;
                        default:
                            throw new Exception($"No event type defined for: {evt.ZebraMQTTResponses.Command}");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "[ConfigureMqttClient] Unexpected Exception");
                }
            }

            await Task.CompletedTask;
        };

        MqttClient.ApplicationMessageSkippedAsync += async (arg) =>
        {
            logger.LogWarning("[ConfigureMqttClient] Skipped Message Id {Id}", arg.ApplicationMessage.Id);

        };
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("[StartAsync] Start Mqtt Client with Client Id {ClientId}", options.ClientOptions.ClientId);
        await MqttClient.StartAsync(options);

        logger.LogInformation("[StartAsync] Started Mqtt Client with Client Id {ClientId}", options.ClientOptions.ClientId);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("[StopAsync] Stop Mqtt Client with Client Id {ClientId}", options.ClientOptions.ClientId);
        await MqttClient.StopAsync();

        logger.LogDebug("[StopAsync] Stopped Mqtt Client with Client Id {ClientId}", options.ClientOptions.ClientId);

    }

    public async Task EnqueueAsync(ManagedMqttApplicationMessage applicationMessage)
    {
        try
        {
            logger.LogDebug("[EnqueueAsync] Enqueue Begin - ApplicationMessageId {Id}", applicationMessage.Id);
            await MqttClient.EnqueueAsync(applicationMessage);

            logger.LogDebug("[EnqueueAsync] Enqueue End - ApplicationMessageId {Id}", applicationMessage.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[EnqueueAsync] Unexpected Exceprion");
        }
    }
}
