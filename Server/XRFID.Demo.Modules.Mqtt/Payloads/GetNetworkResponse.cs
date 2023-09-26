using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xerum.XFramework.MassTransit;
using XRFID.Demo.Modules.Mqtt.Interfaces;

namespace XRFID.Demo.Modules.Mqtt.Payloads;


/// <summary>
/// Reader network information
/// </summary>
[DataContract]
public class GetNetworkResponse : IMqttResponseData, IRequestMessage
{
    /// <summary>
    /// Host name of the reader
    /// </summary>
    /// <value>Host name of the reader</value>
    [DataMember(Name = "hostName", EmitDefaultValue = false)]
    [JsonPropertyName("hostName")]
    public string HostName { get; set; }

    /// <summary>
    /// IP address of the reader
    /// </summary>
    /// <value>IP address of the reader</value>
    [DataMember(Name = "ipAddress", EmitDefaultValue = false)]
    [JsonPropertyName("ipAddress")]
    public string IpAddress { get; set; }

    /// <summary>
    /// IP address of the gateway
    /// </summary>
    /// <value>IP address of the gateway</value>
    [DataMember(Name = "gatewayAddress", EmitDefaultValue = false)]
    [JsonPropertyName("gatewayAddress")]
    public string GatewayAddress { get; set; }

    /// <summary>
    /// Subnet mask for the network adapter
    /// </summary>
    /// <value>Subnet mask for the network adapter</value>
    [DataMember(Name = "subnetMask", EmitDefaultValue = false)]
    [JsonPropertyName("subnetMask")]
    public string SubnetMask { get; set; }

    /// <summary>
    /// IP address of the DNS server
    /// </summary>
    /// <value>IP address of the DNS server</value>
    [DataMember(Name = "dnsAddress", EmitDefaultValue = false)]
    [JsonPropertyName("dnsAddress")]
    public string DnsAddress { get; set; }

    /// <summary>
    /// A value indicating DHCP configuration
    /// </summary>
    /// <value>A value indicating DHCP configuration</value>
    [DataMember(Name = "dhcp", EmitDefaultValue = false)]
    [JsonPropertyName("dhcp")]
    public bool? Dhcp { get; set; }

    /// <summary>
    /// MAC address of the reader
    /// </summary>
    /// <value>MAC address of the reader</value>
    [DataMember(Name = "macAddress", EmitDefaultValue = false)]
    [JsonPropertyName("macAddress")]
    public string MacAddress { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class GetNetworkResponse {\n");
        sb.Append("  HostName: ").Append(HostName).Append("\n");
        sb.Append("  IpAddress: ").Append(IpAddress).Append("\n");
        sb.Append("  GatewayAddress: ").Append(GatewayAddress).Append("\n");
        sb.Append("  SubnetMask: ").Append(SubnetMask).Append("\n");
        sb.Append("  DnsAddress: ").Append(DnsAddress).Append("\n");
        sb.Append("  Dhcp: ").Append(Dhcp).Append("\n");
        sb.Append("  MacAddress: ").Append(MacAddress).Append("\n");
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
