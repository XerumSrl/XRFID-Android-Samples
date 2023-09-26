using Xerum.XFramework.Common.Enums;

namespace XRFID.Demo.Server.Entities;

public class Reader : AuditEntity
{
    public string Ip { get; set; } = "127.0.0.1";

    public ReaderStatus Status { get; set; } = ReaderStatus.Disconnected;

    public Guid CorrelationId { get; set; }

    public Guid? ActiveMovementId { get; set; }

    public string? GpoConfiguration { get; set; }

    public string? Uid { get; set; }

    public string? MacAddress { get; set; }

    public string? Model { get; set; }

    public string? Version { get; set; }

    public string? SerialNumber { get; set; }

    public string? ReaderOS { get; set; }
}
