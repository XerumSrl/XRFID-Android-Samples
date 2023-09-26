using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using XRFID.Demo.Modules.Mqtt.Interfaces;

namespace XRFID.Demo.Modules.Mqtt.Events;


/// <summary>
/// Asynchronous custom Events from userapps
/// </summary>
[DataContract]
public class Userapp : IMqttMessageData
{
    /// <summary>
    /// Host name of the reader
    /// </summary>
    /// <value>Host name of the reader</value>
    [DataMember(Name = "hostName", EmitDefaultValue = false)]
    [JsonPropertyName("hostName")]
    public string HostName { get; set; }

    /// <summary>
    /// RAW event string from userapp
    /// </summary>
    /// <value>RAW event string from userapp</value>
    [DataMember(Name = "event", EmitDefaultValue = false)]
    [JsonPropertyName("event")]
    public string _Event { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class Userapp {\n");
        sb.Append("  _Event: ").Append(_Event).Append("\n");
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
