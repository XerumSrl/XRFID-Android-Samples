using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XRFID.Demo.Modules.Mqtt.Events;


/// <summary>
/// System Heartbeat Fields
/// </summary>
[DataContract]
public class HeartbeatSystem
{
    /// <summary>
    /// Gets or Sets Cpu
    /// </summary>
    [DataMember(Name = "cpu", EmitDefaultValue = false)]
    [JsonPropertyName("cpu")]
    public HeartbeatSystemCpu Cpu { get; set; }

    /// <summary>
    /// Gets or Sets Ram
    /// </summary>
    [DataMember(Name = "ram", EmitDefaultValue = false)]
    [JsonPropertyName("ram")]
    public HeartbeatSystemRam Ram { get; set; }

    /// <summary>
    /// Gets or Sets Flash
    /// </summary>
    [DataMember(Name = "flash", EmitDefaultValue = false)]
    [JsonPropertyName("flash")]
    public HeartbeatSystemFlash Flash { get; set; }

    /// <summary>
    /// Reader uptime (%d days hours:min:sec)
    /// </summary>
    /// <value>Reader uptime (%d days hours:min:sec)</value>
    [DataMember(Name = "uptime", EmitDefaultValue = false)]
    [JsonPropertyName("uptime")]
    public string Uptime { get; set; }

    /// <summary>
    /// Gets or Sets Ntp
    /// </summary>
    [DataMember(Name = "ntp", EmitDefaultValue = false)]
    [JsonPropertyName("ntp")]
    public dynamic Ntp { get; set; }

    /// <summary>
    /// Gets or Sets Temperature
    /// </summary>
    [DataMember(Name = "temperature", EmitDefaultValue = false)]
    [JsonPropertyName("temperature")]
    public HeartbeatSystemTemperature Temperature { get; set; }

    /// <summary>
    /// Reader System Time
    /// </summary>
    /// <value>Reader System Time</value>
    [DataMember(Name = "systemTime", EmitDefaultValue = false)]
    [JsonPropertyName("systemTime")]
    public string SystemTime { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class HeartbeatSystem {\n");
        sb.Append("  Cpu: ").Append(Cpu).Append("\n");
        sb.Append("  Ram: ").Append(Ram).Append("\n");
        sb.Append("  Flash: ").Append(Flash).Append("\n");
        sb.Append("  Uptime: ").Append(Uptime).Append("\n");
        sb.Append("  Ntp: ").Append(Ntp).Append("\n");
        sb.Append("  Temperature: ").Append(Temperature).Append("\n");
        sb.Append("  SystemTime: ").Append(SystemTime).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    /// <summary>
    /// Get the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public string ToJson()
    {
        return JsonSerializer.Serialize(this, this.GetType());
    }

}
