using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XRFID.Demo.Modules.Mqtt.Events;


/// <summary>
/// Total Flash utilization
/// </summary>
[DataContract]
public class HeartbeatSystemFlash
{
    /// <summary>
    /// Gets or Sets Platform
    /// </summary>
    [DataMember(Name = "platform", EmitDefaultValue = false)]
    [JsonPropertyName("platform")]
    public HeartbeatSystemFlashPlatform Platform { get; set; }

    /// <summary>
    /// Gets or Sets ReaderConfig
    /// </summary>
    [DataMember(Name = "readerConfig", EmitDefaultValue = false)]
    [JsonPropertyName("readerConfig")]
    public HeartbeatSystemFlashPlatform ReaderConfig { get; set; }

    /// <summary>
    /// Gets or Sets ReaderData
    /// </summary>
    [DataMember(Name = "readerData", EmitDefaultValue = false)]
    [JsonPropertyName("readerData")]
    public HeartbeatSystemFlashPlatform ReaderData { get; set; }

    /// <summary>
    /// Gets or Sets RootFileSystem
    /// </summary>
    [DataMember(Name = "rootFileSystem", EmitDefaultValue = false)]
    [JsonPropertyName("rootFileSystem")]
    public HeartbeatSystemFlashPlatform RootFileSystem { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class HeartbeatSystemFlash {\n");
        sb.Append("  Platform: ").Append(Platform).Append("\n");
        sb.Append("  ReaderConfig: ").Append(ReaderConfig).Append("\n");
        sb.Append("  ReaderData: ").Append(ReaderData).Append("\n");
        sb.Append("  RootFileSystem: ").Append(RootFileSystem).Append("\n");
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
