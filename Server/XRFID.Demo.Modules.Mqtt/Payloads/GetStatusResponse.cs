using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xerum.XFramework.MassTransit;
using XRFID.Demo.Modules.Mqtt.Interfaces;

namespace XRFID.Demo.Modules.Mqtt.Payloads;


/// <summary>
/// Retrieves reader operational statistics for read points
/// </summary>
[DataContract]
public class GetStatusResponse : IMqttResponseData, IRequestMessage
{
    /// <summary>
    /// Host name of the reader
    /// </summary>
    /// <value>Host name of the reader</value>
    [DataMember(Name = "hostName", EmitDefaultValue = false)]
    [JsonPropertyName("hostName")]
    public string HostName { get; set; }

    /// <summary>
    /// Duration the reader has been powered on
    /// </summary>
    /// <value>Duration the reader has been powered on</value>
    [DataMember(Name = "uptime", EmitDefaultValue = false)]
    [JsonPropertyName("uptime")]
    public string Uptime { get; set; }

    /// <summary>
    /// ISO 8601 formatted time on the reader
    /// </summary>
    /// <value>ISO 8601 formatted time on the reader</value>
    [DataMember(Name = "systemTime", EmitDefaultValue = false)]
    [JsonPropertyName("systemTime")]
    public DateTime? SystemTime { get; set; }

    /// <summary>
    /// Gets or Sets Ram
    /// </summary>
    [DataMember(Name = "ram", EmitDefaultValue = false)]
    [JsonPropertyName("ram")]
    public MemoryStats Ram { get; set; }

    /// <summary>
    /// Gets or Sets Flash
    /// </summary>
    [DataMember(Name = "flash", EmitDefaultValue = false)]
    [JsonPropertyName("flash")]
    public ReaderFlashMemory Flash { get; set; }

    /// <summary>
    /// Gets or Sets Cpu
    /// </summary>
    [DataMember(Name = "cpu", EmitDefaultValue = false)]
    [JsonPropertyName("cpu")]
    public CpuStats Cpu { get; set; }

    /// <summary>
    /// The status of the radio connection
    /// </summary>
    /// <value>The status of the radio connection</value>
    [DataMember(Name = "radioConnection", EmitDefaultValue = false)]
    [JsonPropertyName("radioConnection")]
    public string RadioConnection { get; set; }

    /// <summary>
    /// Gets or Sets Antennas
    /// </summary>
    [DataMember(Name = "antennas", EmitDefaultValue = false)]
    [JsonPropertyName("antennas")]
    public GetStatusResponseAntennas Antennas { get; set; }

    /// <summary>
    /// Current Reader Temperature (in degrees centigrade)
    /// </summary>
    /// <value>Current Reader Temperature (in degrees centigrade)</value>
    [DataMember(Name = "temperature", EmitDefaultValue = false)]
    [JsonPropertyName("temperature")]
    public int? Temperature { get; set; }

    /// <summary>
    /// Status of the radio activity
    /// </summary>
    /// <value>Status of the radio activity</value>
    [DataMember(Name = "radioActivitiy", EmitDefaultValue = false)]
    [JsonPropertyName("radioActivitiy")]
    public string RadioActivitiy { get; set; }

    /// <summary>
    /// The source of power for the reader
    /// </summary>
    /// <value>The source of power for the reader</value>
    [DataMember(Name = "powerSource", EmitDefaultValue = false)]
    [JsonPropertyName("powerSource")]
    public string PowerSource { get; set; }

    /// <summary>
    /// How the power supplied to the reader is negotiated Only present if powerSource is POE or POE+
    /// </summary>
    /// <value>How the power supplied to the reader is negotiated Only present if powerSource is POE or POE+</value>
    [DataMember(Name = "powerNegotiation", EmitDefaultValue = false)]
    [JsonPropertyName("powerNegotiation")]
    public string PowerNegotiation { get; set; }

    /// <summary>
    /// Gets or Sets Ntp
    /// </summary>
    [DataMember(Name = "ntp", EmitDefaultValue = false)]
    [JsonPropertyName("ntp")]
    public OneOfgetStatusResponseNtp Ntp { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class GetStatusResponse {\n");
        sb.Append("  Uptime: ").Append(Uptime).Append("\n");
        sb.Append("  SystemTime: ").Append(SystemTime).Append("\n");
        sb.Append("  Ram: ").Append(Ram).Append("\n");
        sb.Append("  Flash: ").Append(Flash).Append("\n");
        sb.Append("  Cpu: ").Append(Cpu).Append("\n");
        sb.Append("  RadioConnection: ").Append(RadioConnection).Append("\n");
        sb.Append("  Antennas: ").Append(Antennas).Append("\n");
        sb.Append("  Temperature: ").Append(Temperature).Append("\n");
        sb.Append("  RadioActivitiy: ").Append(RadioActivitiy).Append("\n");
        sb.Append("  PowerSource: ").Append(PowerSource).Append("\n");
        sb.Append("  PowerNegotiation: ").Append(PowerNegotiation).Append("\n");
        sb.Append("  Ntp: ").Append(Ntp).Append("\n");
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
