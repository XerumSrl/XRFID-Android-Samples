using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using XRFID.Demo.Modules.Mqtt.Interfaces;

namespace XRFID.Demo.Modules.Mqtt.Events;


/// <summary>
/// current firmware update progress
/// </summary>
[DataContract]
public class FirmwareUpdateProgress : IMqttMessageData
{

    /// <summary>
    /// Host name of the reader
    /// </summary>
    /// <value>Host name of the reader</value>
    [DataMember(Name = "hostName", EmitDefaultValue = false)]
    [JsonPropertyName("hostName")]
    public string HostName { get; set; }
    /// <summary>
    /// current firmware update status
    /// </summary>
    /// <value>current firmware update status</value>
    [DataMember(Name = "status", EmitDefaultValue = false)]
    [JsonPropertyName("status")]
    public string Status { get; set; }

    /// <summary>
    /// current partition update progress percentage
    /// </summary>
    /// <value>current partition update progress percentage</value>
    [DataMember(Name = "imageDownloadProgress", EmitDefaultValue = false)]
    [JsonPropertyName("imageDownloadProgress")]
    public decimal? ImageDownloadProgress { get; set; }

    /// <summary>
    /// overall update progress percentage
    /// </summary>
    /// <value>overall update progress percentage</value>
    [DataMember(Name = "overallUpdateProgress", EmitDefaultValue = false)]
    [JsonPropertyName("overallUpdateProgress")]
    public decimal? OverallUpdateProgress { get; set; }

    /// <summary>
    /// Gets or Sets UpdateProgressDetails
    /// </summary>
    [DataMember(Name = "updateProgressDetails", EmitDefaultValue = false)]
    [JsonPropertyName("updateProgressDetails")]
    public OneOffirmwareUpdateProgressUpdateProgressDetails UpdateProgressDetails { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class FirmwareUpdateProgress {\n");
        sb.Append("  Status: ").Append(Status).Append("\n");
        sb.Append("  ImageDownloadProgress: ").Append(ImageDownloadProgress).Append("\n");
        sb.Append("  OverallUpdateProgress: ").Append(OverallUpdateProgress).Append("\n");
        sb.Append("  UpdateProgressDetails: ").Append(UpdateProgressDetails).Append("\n");
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
