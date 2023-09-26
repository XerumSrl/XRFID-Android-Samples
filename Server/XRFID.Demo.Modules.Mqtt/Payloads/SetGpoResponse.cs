using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xerum.XFramework.MassTransit;
using XRFID.Demo.Modules.Mqtt.Interfaces;

namespace XRFID.Demo.Modules.Mqtt.Payloads;


/// <summary>
/// Updates GPO port state
/// </summary>
[DataContract]
public class SetGpoResponse : IMqttResponseData, IRequestMessage
{

    /// <summary>
    /// Host name of the reader
    /// </summary>
    /// <value>Host name of the reader</value>
    [DataMember(Name = "hostName", EmitDefaultValue = false)]
    [JsonPropertyName("hostName")]
    public string HostName { get; set; }

    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class SetGpoResponse {\n");
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
