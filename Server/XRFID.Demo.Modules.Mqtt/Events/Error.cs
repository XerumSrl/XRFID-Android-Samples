using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using XRFID.Demo.Modules.Mqtt.Interfaces;

namespace XRFID.Demo.Modules.Mqtt.Events;


/// <summary>
/// Error messages:  Reader Gateway:  cpu:     CPU utilization @ %d%  ram:     RAM utilization @ %d%  flash:     FLASH utilization @ %d%  ntp:     NTP synchronization failed offset %f and reach %d  reader_gateway:     zmq polling failed in %s: %d     data endpoint disconnected %s (when data path is configured different from cmd/rsp)     data retention queue full (when data path is configured different from cmd/rsp)  Radio Control:  Antennas:     Antenna Disconnected on Port %d     Antenna Reconnected on Port %d  Database:      Failed to stop NGE before resetting database (%d)     Failed to reset database while resetting database (%d)     Failed to restart NGE while resetting database (%d)     Database full, resetting database  Temperature:         Ambient Temperature Critical @ %d C     PA Temperature Critical @ %d C  Radio Failures:     Tx (PA) Failure on Port %d     NGE Stopped due to error: %s     NGE Error: %s  NGE API:     Error parsing tag info packet  radio_control:     zmq polling error failed in radio.cpp: %s
/// </summary>
[DataContract]
public class Error : IMqttMessageData
{
    /// <summary>
    /// Host name of the reader
    /// </summary>
    /// <value>Host name of the reader</value>
    [DataMember(Name = "hostName", EmitDefaultValue = false)]
    [JsonPropertyName("hostName")]
    public string HostName { get; set; }
    /// <summary>
    /// Error message
    /// </summary>
    /// <value>Error message</value>
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
        sb.Append("class Error {\n");
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
