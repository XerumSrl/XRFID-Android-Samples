using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using XRFID.Demo.Modules.Mqtt.Interfaces;

namespace XRFID.Demo.Modules.Mqtt.Events;


/// <summary>
/// Warning messages:  Reader Gateway:  cpu:     CPU utilization @ %d%  ram:     RAM utilization @ %d%  flash:     FLASH utilization @ %d%  ntp:     NTP synchronization failed offset %f and reach %d  reader_gateway:     (not defined yet)  Radio:  Temperature:     Ambient Temperature High @ %d C     PA Temperature High @ %d C  Database:     Database warning: %d%% full. Will reset at %s (or earlier)     Resetting database  NGE API:     API Error: 0x%08x
/// </summary>
[DataContract]
public class Warning : IMqttMessageData
{
    /// <summary>
    /// Host name of the reader
    /// </summary>
    /// <value>Host name of the reader</value>
    [DataMember(Name = "hostName", EmitDefaultValue = false)]
    [JsonPropertyName("hostName")]
    public string HostName { get; set; }
    /// <summary>
    /// Warning message
    /// </summary>
    /// <value>Warning message</value>
    [DataMember(Name = "message", EmitDefaultValue = false)]
    [JsonPropertyName("message")]
    public string Message { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class Warning {\n");
        sb.Append("  Message: ").Append(Message).Append("\n");
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
