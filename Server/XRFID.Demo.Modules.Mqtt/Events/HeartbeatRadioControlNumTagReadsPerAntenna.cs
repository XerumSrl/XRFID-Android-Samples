using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XRFID.Demo.Modules.Mqtt.Events;


/// <summary>
/// Tags Read Per Antenna
/// </summary>
[DataContract]
public class HeartbeatRadioControlNumTagReadsPerAntenna
{
    /// <summary>
    /// Tags Read on Antenna 1
    /// </summary>
    /// <value>Tags Read on Antenna 1</value>
    [DataMember(Name = "1", EmitDefaultValue = false)]
    [JsonPropertyName("1")]
    public decimal? _1 { get; set; }

    /// <summary>
    /// Tags Read on Antenna 2
    /// </summary>
    /// <value>Tags Read on Antenna 2</value>
    [DataMember(Name = "2", EmitDefaultValue = false)]
    [JsonPropertyName("2")]
    public decimal? _2 { get; set; }

    /// <summary>
    /// Tags Read on Antenna 3
    /// </summary>
    /// <value>Tags Read on Antenna 3</value>
    [DataMember(Name = "3", EmitDefaultValue = false)]
    [JsonPropertyName("3")]
    public decimal? _3 { get; set; }

    /// <summary>
    /// Tags Read on Antenna 4
    /// </summary>
    /// <value>Tags Read on Antenna 4</value>
    [DataMember(Name = "4", EmitDefaultValue = false)]
    [JsonPropertyName("4")]
    public decimal? _4 { get; set; }

    /// <summary>
    /// Tags Read on Antenna 5
    /// </summary>
    /// <value>Tags Read on Antenna 5</value>
    [DataMember(Name = "5", EmitDefaultValue = false)]
    [JsonPropertyName("5")]
    public decimal? _5 { get; set; }

    /// <summary>
    /// Tags Read on Antenna 6
    /// </summary>
    /// <value>Tags Read on Antenna 6</value>
    [DataMember(Name = "6", EmitDefaultValue = false)]
    [JsonPropertyName("6")]
    public decimal? _6 { get; set; }

    /// <summary>
    /// Tags Read on Antenna 7
    /// </summary>
    /// <value>Tags Read on Antenna 7</value>
    [DataMember(Name = "7", EmitDefaultValue = false)]
    [JsonPropertyName("7")]
    public decimal? _7 { get; set; }

    /// <summary>
    /// Tags Read on Antenna 8
    /// </summary>
    /// <value>Tags Read on Antenna 8</value>
    [DataMember(Name = "8", EmitDefaultValue = false)]
    [JsonPropertyName("8")]
    public decimal? _8 { get; set; }

    /// <summary>
    /// Tags Read on Antenna 9 (only applicable to ATR7000)
    /// </summary>
    /// <value>Tags Read on Antenna 9 (only applicable to ATR7000)</value>
    [DataMember(Name = "9", EmitDefaultValue = false)]
    [JsonPropertyName("9")]
    public decimal? _9 { get; set; }

    /// <summary>
    /// Tags Read on Antenna 10 (only applicable to ATR7000)
    /// </summary>
    /// <value>Tags Read on Antenna 10 (only applicable to ATR7000)</value>
    [DataMember(Name = "10", EmitDefaultValue = false)]
    [JsonPropertyName("10")]
    public decimal? _10 { get; set; }

    /// <summary>
    /// Tags Read on Antenna 11 (only applicable to ATR7000)
    /// </summary>
    /// <value>Tags Read on Antenna 11 (only applicable to ATR7000)</value>
    [DataMember(Name = "11", EmitDefaultValue = false)]
    [JsonPropertyName("11")]
    public decimal? _11 { get; set; }

    /// <summary>
    /// Tags Read on Antenna 12 (only applicable to ATR7000)
    /// </summary>
    /// <value>Tags Read on Antenna 12 (only applicable to ATR7000)</value>
    [DataMember(Name = "12", EmitDefaultValue = false)]
    [JsonPropertyName("12")]
    public decimal? _12 { get; set; }

    /// <summary>
    /// Tags Read on Antenna 13 (only applicable to ATR7000)
    /// </summary>
    /// <value>Tags Read on Antenna 13 (only applicable to ATR7000)</value>
    [DataMember(Name = "13", EmitDefaultValue = false)]
    [JsonPropertyName("13")]
    public decimal? _13 { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class HeartbeatRadioControlNumTagReadsPerAntenna {\n");
        sb.Append("  _1: ").Append(_1).Append("\n");
        sb.Append("  _2: ").Append(_2).Append("\n");
        sb.Append("  _3: ").Append(_3).Append("\n");
        sb.Append("  _4: ").Append(_4).Append("\n");
        sb.Append("  _5: ").Append(_5).Append("\n");
        sb.Append("  _6: ").Append(_6).Append("\n");
        sb.Append("  _7: ").Append(_7).Append("\n");
        sb.Append("  _8: ").Append(_8).Append("\n");
        sb.Append("  _9: ").Append(_9).Append("\n");
        sb.Append("  _10: ").Append(_10).Append("\n");
        sb.Append("  _11: ").Append(_11).Append("\n");
        sb.Append("  _12: ").Append(_12).Append("\n");
        sb.Append("  _13: ").Append(_13).Append("\n");
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
