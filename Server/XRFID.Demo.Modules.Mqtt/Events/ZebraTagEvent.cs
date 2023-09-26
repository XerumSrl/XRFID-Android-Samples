using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XRFID.Demo.Modules.Mqtt.Events;

/// <summary>
/// Asynchronous Tag Events
/// </summary>
[DataContract]
public class ZebraTagEvent
{

    public ZebraTagEvent()
    {
        Timestamp = DateTime.Now;
    }

    /// <summary>
    /// Host name of the reader
    /// </summary>
    /// <value>Host name of the reader</value>
    [DataMember(Name = "hostName", EmitDefaultValue = false)]
    [JsonPropertyName("hostName")]
    [Required]
    public string HostName { get; set; }

    /// <summary>
    /// Event type
    /// </summary>
    /// <value>Event type</value>
    [DataMember(Name = "type", EmitDefaultValue = false)]
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// Event timestamp
    /// </summary>
    /// <value>Event timestamp</value>
    [DataMember(Name = "timestamp", EmitDefaultValue = false)]
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Event Data
    /// </summary>
    /// <value>Event Data</value>
    [DataMember(Name = "data", EmitDefaultValue = false)]
    [JsonPropertyName("data")]
    public dynamic Data { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class AsyncEvents {\n");
        sb.Append("  Type: ").Append(Type).Append("\n");
        sb.Append("  Timestamp: ").Append(Timestamp).Append("\n");
        sb.Append("  Data: ").Append(Data).Append("\n");
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
