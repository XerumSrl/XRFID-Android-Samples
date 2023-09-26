using XRFID.Demo.Modules.Mqtt.Payloads;
using XRFID.Demo.Modules.Mqtt.Publishers;

namespace XRFID.Demo.Server.Utilities;

public class GpoUtility
{
    private readonly IZebraMqttCommandPublisher zebraMqttCommandPublisher;
    private readonly ILogger<GpoUtility> logger;

    public GpoUtility(
        IZebraMqttCommandPublisher zebraMqttCommandPublisher,
        ILogger<GpoUtility> logger)
    {
        this.zebraMqttCommandPublisher = zebraMqttCommandPublisher;
        this.logger = logger;
    }

    public async Task SetGpo(Guid readerId, string hostname, int gpoPort, bool gpoStatus)
    {
        try
        {

            await zebraMqttCommandPublisher.Publish(new RAWMQTTCommands
            {
                ReaderId = readerId,
                Topic = "mcmds",
                HostName = hostname,
                Command = "set_gpo",
                CommandId = Guid.NewGuid().ToString(),
                Payload = new SetGpoCommand
                {
                    Port = gpoPort,
                    State = gpoStatus,
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
        }
    }
}


public class GpoConfiguration
{
    public int GreenLed { get; set; } = 1;
    public int YellowLed { get; set; } = 2;
    public int RedLed { get; set; } = 3;
    public int Buzzer { get; set; } = 4;
}
