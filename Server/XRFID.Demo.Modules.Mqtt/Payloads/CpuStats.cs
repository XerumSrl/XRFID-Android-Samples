using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XRFID.Demo.Modules.Mqtt.Payloads;


/// <summary>
/// System CPU Statistics
/// </summary>
[DataContract]
public class CpuStats
{
    /// <summary>
    /// User processes CPU utilization percentage
    /// </summary>
    /// <value>User processes CPU utilization percentage</value>
    [DataMember(Name = "user", EmitDefaultValue = false)]
    [JsonPropertyName("user")]
    public int? User { get; set; }

    /// <summary>
    /// System processes CPU utilization percentage
    /// </summary>
    /// <value>System processes CPU utilization percentage</value>
    [DataMember(Name = "system", EmitDefaultValue = false)]
    [JsonPropertyName("system")]
    public int? System { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class CpuStats {\n");
        sb.Append("  User: ").Append(User).Append("\n");
        sb.Append("  System: ").Append(System).Append("\n");
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
