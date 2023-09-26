using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XRFID.Demo.Modules.Mqtt.Events;


/// <summary>
/// Total CPU utilization
/// </summary>
[DataContract]
public class HeartbeatSystemCpu
{
    /// <summary>
    /// CPU utilization of System
    /// </summary>
    /// <value>CPU utilization of System</value>
    [DataMember(Name = "system", EmitDefaultValue = false)]
    [JsonPropertyName("system")]
    public decimal? System { get; set; }

    /// <summary>
    /// CPU utilization of user
    /// </summary>
    /// <value>CPU utilization of user</value>
    [DataMember(Name = "user", EmitDefaultValue = false)]
    [JsonPropertyName("user")]
    public decimal? User { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class HeartbeatSystemCpu {\n");
        sb.Append("  System: ").Append(System).Append("\n");
        sb.Append("  User: ").Append(User).Append("\n");
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