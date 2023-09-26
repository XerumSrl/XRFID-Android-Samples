using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xerum.XFramework.Common.Enums;
using Xerum.XFramework.MassTransit;
using XRFID.Demo.Modules.Mqtt.Interfaces;

namespace XRFID.Demo.Modules.Mqtt.Contracts;

/// <summary>
/// TAG Event
/// </summary>
[DataContract]
public class ZebraTagData : IMqttTagData, IRequestMessage
{
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// Host name of the reader
    /// </summary>
    /// <value>Host name of the reader</value>
    [DataMember(Name = "hostName", EmitDefaultValue = false)]
    [JsonPropertyName("hostName")]
    [Required]
    public string HostName { get; set; }

    /// <summary>
    /// Host name of the reader
    /// </summary>
    /// <value>Host name of the reader</value>
    [DataMember(Name = "eventNum", EmitDefaultValue = false)]
    [JsonPropertyName("eventNum")]
    public int? EventNum { get; set; }

    /// <summary>
    /// Host name of the reader
    /// </summary>
    /// <value>Host name of the reader</value>
    [DataMember(Name = "format", EmitDefaultValue = false)]
    [JsonPropertyName("format")]
    public string? Format { get; set; }

    /// <summary>
    /// Host name of the reader
    /// </summary>
    /// <value>Host name of the reader</value>
    [DataMember(Name = "idHex", EmitDefaultValue = false)]
    [JsonPropertyName("idHex")]
    [Required]
    public string IdHex { get; set; }


    /// <summary>
    /// Number of tag reads
    /// </summary>
    /// <value>Number of tag reads</value>
    [DataMember(Name = "reads", EmitDefaultValue = false)]
    [JsonPropertyName("reads")]
    public int? Reads { get; set; }

    /****************************** Aggiunta di nuovi campi *********************************************/

    //ANTENNA
    [DataMember(Name = "antenna", EmitDefaultValue = false)]
    [JsonPropertyName("antenna")]
    public string? Antenna { get; set; }

    /// <summary>
    /// The phase (in degrees) of the inventoried tag.
    /// This value will only be reported if each individual tag read is reported (if reportFilter duration is set to 0). 
    /// Otherwise, it will not be reported.
    /// </summary>
    /// <value>Host name of the reader</value>
    [DataMember(Name = "phase", EmitDefaultValue = false)]
    [JsonPropertyName("phase")]
    public string? Phase { get; set; }

    /// <summary>
    /// User Memory bits of the inventoried tag as a hex string.
    /// </summary>
    /// <value>Host name of the reader</value>
    [DataMember(Name = "USER", EmitDefaultValue = false)]
    [JsonPropertyName("USER")]
    public string? UserMemory { get; set; }

    /// <summary>
    /// Channel (in MHz) the reader was using when the tag was inventoried. 
    /// This value will only be reported if each individual tag read is reported (if reportFilter duration is set to 0). 
    /// Otherwise, it will not be reported.
    /// </summary>
    /// <value>Channel of the Tag</value>
    [DataMember(Name = "channel", EmitDefaultValue = false)]
    [JsonPropertyName("channel")]
    public string? Channel { get; set; }

    /// <summary>
    /// PC bits of the inventoried tag as a hex string.
    /// </summary>
    /// <value>Pc of the Tag</value>
    [DataMember(Name = "PC", EmitDefaultValue = false)]
    [JsonPropertyName("PC")]
    public string? Pc { get; set; }

    /// <summary>
    /// XPC bits of the inventoried tag, if present, as a hex string.
    /// </summary>
    /// <value>Xpc of the Tag</value>
    [DataMember(Name = "XPC", EmitDefaultValue = false)]
    [JsonPropertyName("XPC")]
    public string? Xpc { get; set; }

    /// <summary>
    /// CRC bits of the inventoried tag as a hex string.
    /// </summary>
    /// <value>Crc of the Tag</value>
    [DataMember(Name = "CRC", EmitDefaultValue = false)]
    [JsonPropertyName("CRC")]
    public string? Crc { get; set; }

    /// <summary>
    /// TID bits of the inventoried tag as a hex string.
    /// </summary>
    /// <value>Tid of the Tag</value>
    [DataMember(Name = "TID", EmitDefaultValue = false)]
    [JsonPropertyName("TID")]
    public string? Tid { get; set; }

    /// <summary>
    /// It will report the rssi (in dbm) of the inventoried tag.
    /// </summary>
    /// <value>PeakRssi of the Tag</value>
    [DataMember(Name = "peakRssi", EmitDefaultValue = false)]
    [JsonPropertyName("peakRssi")]
    public short? PeakRssi { get; set; }

    /// <summary>
    /// Specifies user defined field which will be reported as part of the tag meta data.
    /// </summary>
    /// <value>UserDefined of the Tag</value>
    [DataMember(Name = "userDefined", EmitDefaultValue = false)]
    [JsonPropertyName("userDefined")]
    public string? UserDefined { get; set; }

    public ItemStatus EventStatus { get; set; } = ItemStatus.None;

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class TagData {\n");
        sb.Append("  EventNum: ").Append(EventNum).Append("\n");
        sb.Append("  Format: ").Append(Format).Append("\n");
        sb.Append("  IdHex: ").Append(IdHex).Append("\n");
        sb.Append("  CRC: ").Append(Crc).Append("\n");
        sb.Append("  PC: ").Append(Pc).Append("\n");
        sb.Append("  XPC: ").Append(Xpc).Append("\n");
        sb.Append("  TID: ").Append(Tid).Append("\n");
        sb.Append("  USER: ").Append(UserMemory).Append("\n");
        sb.Append("  channel: ").Append(Channel).Append("\n");
        sb.Append("  phase: ").Append(Phase).Append("\n");
        sb.Append("  userDefined: ").Append(UserDefined).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    public virtual string ToJson()
    {
        return JsonSerializer.Serialize(this, this.GetType());
    }
}
