using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xerum.XFramework.MassTransit;
using XRFID.Demo.Modules.Mqtt.Interfaces;

namespace XRFID.Demo.Modules.Mqtt.Payloads;


/// <summary>
/// 
/// </summary>
[DataContract]
public class RAWMQTTCommands : IRequestMessage, IRAWCommand
{
    /// <summary>
    /// Gets or Sets ReaderId
    /// </summary>
    [DataMember(Name = "reader_id", EmitDefaultValue = false)]
    [JsonPropertyName("reader_id")]
    [JsonIgnore]
    public Guid ReaderId { get; set; }

    /// <summary>
    /// Gets or Sets HostName
    /// </summary>
    [DataMember(Name = "hostname", EmitDefaultValue = false)]
    [JsonPropertyName("hostname")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Required]
    public string? HostName { get; set; }

    /// <summary>
    /// Gets or Sets Topic
    /// </summary>
    [DataMember(Name = "topic", EmitDefaultValue = false)]
    [JsonPropertyName("topic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Required]
    public string? Topic { get; set; }

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
        sb.Append("class RAWMQTTCommands {\n");
        sb.Append("  Command: ").Append(Command).Append("\n");
        sb.Append("  CommandId: ").Append(CommandId).Append("\n");
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
