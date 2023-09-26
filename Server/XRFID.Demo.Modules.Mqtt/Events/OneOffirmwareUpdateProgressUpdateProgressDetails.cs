using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

namespace XRFID.Demo.Modules.Mqtt.Events;


/// <summary>
/// 
/// </summary>
[DataContract]
public class OneOffirmwareUpdateProgressUpdateProgressDetails
{

    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class OneOffirmwareUpdateProgressUpdateProgressDetails {\n");
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
