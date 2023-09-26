using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xerum.XFramework.MassTransit;
using XRFID.Demo.Modules.Mqtt.Interfaces;

namespace XRFID.Demo.Modules.Mqtt.Payloads;


/// <summary>
/// Reader version information
/// </summary>
[DataContract]
public class GetVersionResponse : IMqttResponseData, IRequestMessage
{
    /// <summary>
    /// Host name of the reader
    /// </summary>
    /// <value>Host name of the reader</value>
    [DataMember(Name = "hostName", EmitDefaultValue = false)]
    [JsonPropertyName("hostName")]
    public string HostName { get; set; }

    /// <summary>
    /// Reader software version
    /// </summary>
    /// <value>Reader software version</value>
    [DataMember(Name = "readerApplication", EmitDefaultValue = false)]
    [JsonPropertyName("readerApplication")]
    public string ReaderApplication { get; set; }

    /// <summary>
    /// API to the radio engine
    /// </summary>
    /// <value>API to the radio engine</value>
    [DataMember(Name = "radioApi", EmitDefaultValue = false)]
    [JsonPropertyName("radioApi")]
    public string RadioApi { get; set; }

    /// <summary>
    /// Firmware running on the radio
    /// </summary>
    /// <value>Firmware running on the radio</value>
    [DataMember(Name = "radioFirmware", EmitDefaultValue = false)]
    [JsonPropertyName("radioFirmware")]
    public string RadioFirmware { get; set; }

    /// <summary>
    /// Reader radio control version
    /// </summary>
    /// <value>Reader radio control version</value>
    [DataMember(Name = "radioControlApplication", EmitDefaultValue = false)]
    [JsonPropertyName("radioControlApplication")]
    public string RadioControlApplication { get; set; }

    /// <summary>
    /// Reader operating system version
    /// </summary>
    /// <value>Reader operating system version</value>
    [DataMember(Name = "readerOS", EmitDefaultValue = false)]
    [JsonPropertyName("readerOS")]
    public string ReaderOS { get; set; }

    /// <summary>
    /// Hardware version of the reader
    /// </summary>
    /// <value>Hardware version of the reader</value>
    [DataMember(Name = "readerHardware", EmitDefaultValue = false)]
    [JsonPropertyName("readerHardware")]
    public string ReaderHardware { get; set; }

    /// <summary>
    /// Reader boot loader version
    /// </summary>
    /// <value>Reader boot loader version</value>
    [DataMember(Name = "readerBootLoader", EmitDefaultValue = false)]
    [JsonPropertyName("readerBootLoader")]
    public string ReaderBootLoader { get; set; }

    /// <summary>
    /// Reader root file system version
    /// </summary>
    /// <value>Reader root file system version</value>
    [DataMember(Name = "readerFileSystem", EmitDefaultValue = false)]
    [JsonPropertyName("readerFileSystem")]
    public string ReaderFileSystem { get; set; }

    /// <summary>
    /// Reader cloud agent version
    /// </summary>
    /// <value>Reader cloud agent version</value>
    [DataMember(Name = "cloudAgentApplication", EmitDefaultValue = false)]
    [JsonPropertyName("cloudAgentApplication")]
    public string CloudAgentApplication { get; set; }

    /// <summary>
    /// FPGA running on radio (only applicable to ATR7000)
    /// </summary>
    /// <value>FPGA running on radio (only applicable to ATR7000)</value>
    [DataMember(Name = "fpga", EmitDefaultValue = false)]
    [JsonPropertyName("fpga")]
    public string Fpga { get; set; }


    /// <summary>
    /// Reader serial number
    /// </summary>
    /// <value>Reader serial number</value>
    [DataMember(Name = "serialNumber", EmitDefaultValue = false)]
    [JsonPropertyName("serialNumber")]
    public string SerialNumber { get; set; }

    /// <summary>
    /// Reader model
    /// </summary>
    /// <value>Reader model</value>
    [DataMember(Name = "model", EmitDefaultValue = false)]
    [JsonPropertyName("model")]
    public string Model { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class GetVersionResponse {\n");
        sb.Append("  ReaderApplication: ").Append(ReaderApplication).Append("\n");
        sb.Append("  RadioApi: ").Append(RadioApi).Append("\n");
        sb.Append("  RadioFirmware: ").Append(RadioFirmware).Append("\n");
        sb.Append("  RadioControlApplication: ").Append(RadioControlApplication).Append("\n");
        sb.Append("  ReaderOS: ").Append(ReaderOS).Append("\n");
        sb.Append("  ReaderHardware: ").Append(ReaderHardware).Append("\n");
        sb.Append("  ReaderBootLoader: ").Append(ReaderBootLoader).Append("\n");
        sb.Append("  ReaderFileSystem: ").Append(ReaderFileSystem).Append("\n");
        sb.Append("  CloudAgentApplication: ").Append(CloudAgentApplication).Append("\n");
        sb.Append("  SerialNumber: ").Append(SerialNumber).Append("\n");
        sb.Append("  Fpga: ").Append(Fpga).Append("\n");
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
