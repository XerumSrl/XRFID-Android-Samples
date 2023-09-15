using MassTransit;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using System.Text.Json;
using Xerum.XFramework.MassTransit;
using XRFID.Sample.Modules.Mqtt.Interfaces;
using XRFID.Sample.Modules.Mqtt.Payloads;

namespace XRFID.Sample.Modules.Mqtt.Consumers;

/// <summary>
/// Consuma da Backend.ZebraCommandPublisher
/// </summary>
public class ZebraCommandConsumer : IRequestConsumer<RAWMQTTCommands>
{
    private readonly IManagedMqttClientService mqttClientService;
    private readonly ILogger<ZebraCommandConsumer> logger;

    public ZebraCommandConsumer(IManagedMqttClientService mqttClientService, ILogger<ZebraCommandConsumer> logger)
    {
        this.mqttClientService = mqttClientService;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<RAWMQTTCommands> context)
    {
        try
        {
            logger.LogDebug($"ZebraCommandConsumer|RAWMQTTCommands consume begin. CommandId: {context.Message.CommandId}");

            logger.LogDebug($"ZebraCommandConsumer|Topic: {context.Message.Topic}");

            if (string.IsNullOrEmpty(context.Message.Topic) || string.IsNullOrEmpty(context.Message.HostName))
            {
                logger.LogError($"ZebraCommandConsumer|Missing context.Message required property: Topic: {context.Message.Topic} - HostName: {context.Message.HostName}");
                return;
            }
            //essendo un dynamic, serve prima deserializzarlo ottenendo il contenuto
            //per poi serializzarlo nuovamente all'interno del command
            //https://stackoverflow.com/a/72200593
            //var payload = JsonConvert.DeserializeObject(JsonSerializer.Serialize(context.Message.Payload));
            var payload = JsonSerializer.Deserialize<dynamic>((JsonElement)context.Message.Payload);
            var command = JsonSerializer.Serialize(new RAWMQTTCommands
            {
                HostName = null,
                Topic = null,
                Command = context.Message.Command,
                CommandId = context.Message.CommandId,
                Payload = payload ?? new Object(),
            });
            logger.LogDebug(@"ZebraCommandConsumer|Command: {command}", command);

            //chiedo update di alcuni dati direttamente al reader
            var message = new ManagedMqttApplicationMessageBuilder()
                    .WithApplicationMessage(new MqttApplicationMessageBuilder()
                    .WithTopic($"{context.Message.Topic}/{context.Message.HostName}")
                    .WithPayload(command)
                    .Build())
                .Build();

            await mqttClientService.MqttClient.InternalClient.PublishAsync(message.ApplicationMessage);
            //await mqttClient.EnqueueAsync(message);

            logger.LogDebug($"ZebraCommandConsumer|RAWMQTTCommands consume end. CommandId: {context.Message.CommandId}");
        }
        catch (Exception ex)
        {
            logger.LogError($"Message: {ex.Message}\n{ex.InnerException}\n{ex.StackTrace}");
            throw;
        }


    }

}
