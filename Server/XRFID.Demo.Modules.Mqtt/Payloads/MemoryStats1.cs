using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XRFID.Demo.Modules.Mqtt.Payloads;


/// <summary>
/// System memory statistics
/// </summary>
[DataContract]
public class MemoryStats1
{
    /// <summary>
    /// Total RAM in bytes
    /// </summary>
    /// <value>Total RAM in bytes</value>
    [DataMember(Name = "total", EmitDefaultValue = false)]
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    /// <summary>
    /// Free RAM in bytes
    /// </summary>
    /// <value>Free RAM in bytes</value>
    [DataMember(Name = "free", EmitDefaultValue = false)]
    [JsonPropertyName("free")]
    public int? Free { get; set; }

    /// <summary>
    /// Used RAM in bytes
    /// </summary>
    /// <value>Used RAM in bytes</value>
    [DataMember(Name = "used", EmitDefaultValue = false)]
    [JsonPropertyName("used")]
    public int? Used { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class MemoryStats1 {\n");
        sb.Append("  Total: ").Append(Total).Append("\n");
        sb.Append("  Free: ").Append(Free).Append("\n");
        sb.Append("  Used: ").Append(Used).Append("\n");
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
