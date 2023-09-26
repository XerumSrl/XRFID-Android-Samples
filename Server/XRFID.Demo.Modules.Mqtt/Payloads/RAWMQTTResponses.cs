using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XRFID.Demo.Modules.Mqtt.Payloads;


/// <summary>
/// 
/// </summary>
[DataContract]
public class RAWMQTTResponses
{
    /// <summary>
    /// Gets or Sets Command
    /// </summary>
    [DataMember(Name = "command", EmitDefaultValue = false)]
    [JsonPropertyName("command")]
    public string Command { get; set; }

    /// <summary>
    /// Gets or Sets CommandId
    /// </summary>
    [DataMember(Name = "command_id", EmitDefaultValue = false)]
    [JsonPropertyName("command_id")]
    public string CommandId { get; set; }

    /// <summary>
    /// Gets or Sets Response
    /// </summary>
    [DataMember(Name = "response", EmitDefaultValue = false)]
    [JsonPropertyName("response")]
    public string Response { get; set; }

    /// <summary>
    /// Gets or Sets Payload
    /// </summary>
    [DataMember(Name = "payload", EmitDefaultValue = false)]
    [JsonPropertyName("payload")]
    public dynamic Payload { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class RAWMQTTResponses {\n");
        sb.Append("  Command: ").Append(Command).Append("\n");
        sb.Append("  CommandId: ").Append(CommandId).Append("\n");
        sb.Append("  Response: ").Append(Response).Append("\n");
        sb.Append("  Payload: ").Append(Payload).Append("\n");
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
