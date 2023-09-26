using Xerum.XFramework.Common.Dto;
using Xerum.XFramework.Common.Enums;

namespace XRFID.Demo.Common.Dto;

public class ReaderDto : RestEntityDto
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
