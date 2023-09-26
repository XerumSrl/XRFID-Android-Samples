namespace XRFID.Demo.Modules.Mqtt.Interfaces;

public interface IRAWCommand
{
    Guid ReaderId { get; set; }

    string? HostName { get; set; }

    string? Topic { get; set; }

    string Command { get; set; }

    string CommandId { get; set; }

    dynamic Payload { get; set; }

    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    string ToString();

    /// <summary>
    /// Get the JSON string representation of the object
    /// </summary>
    /// <returns>JSON string representation of the object</returns>
    string ToJson();
}
