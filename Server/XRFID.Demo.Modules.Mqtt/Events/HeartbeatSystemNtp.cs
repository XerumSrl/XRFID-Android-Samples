using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XRFID.Demo.Modules.Mqtt.Events;


/// <summary>
/// NTP synchronous status
/// </summary>
[DataContract]
public class HeartbeatSystemNtp
{
    /// <summary>
    /// current NTP offset in ms
    /// </summary>
    /// <value>current NTP offset in ms</value>
    [DataMember(Name = "offset", EmitDefaultValue = false)]
    [JsonPropertyName("offset")]
    public decimal? Offset { get; set; }

    /// <summary>
    /// last 8 NTP sync status
    /// </summary>
    /// <value>last 8 NTP sync status</value>
    [DataMember(Name = "reach", EmitDefaultValue = false)]
    [JsonPropertyName("reach")]
    public decimal? Reach { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class HeartbeatSystemNtp {\n");
        sb.Append("  Offset: ").Append(Offset).Append("\n");
        sb.Append("  Reach: ").Append(Reach).Append("\n");
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
