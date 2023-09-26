using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xerum.XFramework.MassTransit;
using XRFID.Demo.Modules.Mqtt.Interfaces;

namespace XRFID.Demo.Modules.Mqtt.Events;


/// <summary>
/// Heartbeat Event
/// </summary>
[DataContract]
public class Heartbeat : IMqttMessageData, IRequestMessage
{

    /// <summary>
    /// Host name of the reader
    /// </summary>
    /// <value>Host name of the reader</value>
    [DataMember(Name = "hostName", EmitDefaultValue = false)]
    [JsonPropertyName("hostName")]
    public string HostName { get; set; }

    /// <summary>
    /// Gets or Sets RadioControl
    /// </summary>
    [DataMember(Name = "radio_control", EmitDefaultValue = false)]
    [JsonPropertyName("radio_control")]
    public HeartbeatRadioControl RadioControl { get; set; }

    /// <summary>
    /// Gets or Sets ReaderGateway
    /// </summary>
    [DataMember(Name = "reader_gateway", EmitDefaultValue = false)]
    [JsonPropertyName("reader_gateway")]
    public HeartbeatReaderGateway ReaderGateway { get; set; }

    /// <summary>
    /// Gets or Sets System
    /// </summary>
    [DataMember(Name = "system", EmitDefaultValue = false)]
    [JsonPropertyName("system")]
    public HeartbeatSystem System { get; set; }

    /// <summary>
    /// userapp Heartbeat Fields
    /// </summary>
    /// <value>userapp Heartbeat Fields</value>
    [DataMember(Name = "userapps", EmitDefaultValue = false)]
    [JsonPropertyName("userapps")]
    public List<HeartbeatUserapps> Userapps { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class Heartbeat {\n");
        sb.Append("  RadioControl: ").Append(RadioControl).Append("\n");
        sb.Append("  ReaderGateway: ").Append(ReaderGateway).Append("\n");
        sb.Append("  System: ").Append(System).Append("\n");
        sb.Append("  Userapps: ").Append(Userapps).Append("\n");
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
