using System.Runtime.Serialization;
using XRFID.Demo.Modules.Mqtt.Payloads;
using XRFID.Demo.Modules.Mqtt.Publishers;

namespace XRFID.Demo.Server.Utilities;

public class CommandUtility
{
    private readonly IZebraMqttCommandPublisher zebraMqttCommandPublisher;
    private readonly ILogger<CommandUtility> logger;

    public CommandUtility(IZebraMqttCommandPublisher zebraMqttCommandPublisher,
        ILogger<CommandUtility> logger)
    {
        this.zebraMqttCommandPublisher = zebraMqttCommandPublisher;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task StartReading(Guid readerId, string hostName)
    {
        await zebraMqttCommandPublisher.Publish(new RAWMQTTCommands
        {
            ReaderId = readerId,
            Topic = "mcmds",
            HostName = hostName,
            Command = "start",
            CommandId = Guid.NewGuid().ToString(),
            Payload = new VoidPayload()
        });
    }

    /// <inheritdoc/>
    public async Task StopReading(Guid readerId, string hostName)
    {
        await zebraMqttCommandPublisher.Publish(new RAWMQTTCommands
        {
            ReaderId = readerId,
            Topic = "mcmds",
            HostName = hostName,
            Command = "stop",
            CommandId = Guid.NewGuid().ToString(),
            Payload = new VoidPayload()
        });
    }
}

[DataContract]
public class VoidPayload { }