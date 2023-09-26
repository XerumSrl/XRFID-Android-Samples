using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

namespace XRFID.Demo.Modules.Mqtt.Payloads;


/// <summary>
/// 
/// </summary>
[DataContract]
public class OneOfRAWMQTTCommandsPayload
{

    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class OneOfRAWMQTTCommandsPayload {\n");
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
