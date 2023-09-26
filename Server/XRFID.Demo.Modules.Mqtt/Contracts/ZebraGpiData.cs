using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xerum.XFramework.MassTransit;
using XRFID.Demo.Modules.Mqtt.Interfaces;

namespace XRFID.Demo.Modules.Mqtt.Contracts;


/// <summary>
/// GPI Event
/// </summary>
[DataContract]
public class ZebraGpiData : IMqttMessageData, IRequestMessage
{
    public ZebraGpiData()
    {
        Timestamp = DateTime.Now;
        //TimerPlus = new TimerPlus();

        //TimerPlus.Elapsed += TimerPlus_Elapsed;
        //TimerPlus.AutoReset = false;

    }
    private void TimerPlus_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {

        GpioWhatchEnded = true;
        //logger.Debug($"Gpio{gpInId} with value {gpInLogicOn} active for more than {timerPlus.Interval} milliseconds");
        //gpioEventTask.Stop();
    }

    //public TimerPlus TimerPlus;
    public bool GpioWhatchEnded = false;

    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Host name of the reader
    /// </summary>
    /// <value>Host name of the reader</value>
    [DataMember(Name = "hostName", EmitDefaultValue = false)]
    [JsonPropertyName("hostName")]
    [Required]
    public string HostName { get; set; }

    /// <summary>
    /// GPI pin number
    /// </summary>
    /// <value>GPI pin number</value>
    [DataMember(Name = "pin", EmitDefaultValue = false)]
    [JsonPropertyName("pin")]
    [Required]
    public int Pin { get; set; }

    /// <summary>
    /// GPI pin state
    /// </summary>
    /// <value>GPI pin state</value>
    [DataMember(Name = "state", EmitDefaultValue = false)]
    [JsonPropertyName("state")]
    [Required]
    public string State { get; set; }

    public bool Value
    {
        get
        {
            switch (State?.ToLower())
            {
                case "high":
                    return false;
                case "low":
                    return true;
                default:
                    return false;
            }
        }
    }


    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class Gpi {\n");
        sb.Append("  Pin: ").Append(Pin).Append("\n");
        sb.Append("  State: ").Append(State).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    /// <summary>
    /// Get the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public virtual string ToJson()
    {
        return JsonSerializer.Serialize(this, this.GetType());
    }

}
