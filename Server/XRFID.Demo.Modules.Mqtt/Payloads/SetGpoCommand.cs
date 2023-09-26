using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XRFID.Demo.Modules.Mqtt.Payloads;


/// <summary>
/// Updates GPO port state
/// </summary>
[DataContract]
public class SetGpoCommand
{
    /// <summary>
    /// The GPO port ID
    /// </summary>
    /// <value>The GPO port ID</value>
    [DataMember(Name = "port", EmitDefaultValue = false)]
    [JsonPropertyName("port")]
    public int? Port { get; set; }

    /// <summary>
    /// The GPO state signal to send
    /// </summary>
    /// <value>The GPO state signal to send</value>
    [DataMember(Name = "state", EmitDefaultValue = false)]
    [JsonPropertyName("state")]
    public bool? State { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class SetGpoCommand {\n");
        sb.Append("  Port: ").Append(Port).Append("\n");
        sb.Append("  State: ").Append(State).Append("\n");
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
