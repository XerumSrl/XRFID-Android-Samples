using Xerum.XFramework.Common.Enums;

namespace XRFID.Demo.Server.ViewModels;

public class ReaderModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }

    public string Ip { get; set; } = "127.0.0.1";

    public ReaderStatus Status { get; set; } = ReaderStatus.Disconnected;

    public Guid CorrelationId { get; set; }

    public bool HasCorrelation
    {
        get
        {
            return CorrelationId != Guid.Empty;
        }
    }

    public Guid? ActiveMovementId { get; set; }

    public string? GpoConfiguration { get; set; }

    public string? Uid { get; set; }

    public string? MacAddress { get; set; }

    public string? Model { get; set; }

    public string? Version { get; set; }

    public string? SerialNumber { get; set; }

    public string? ReaderOS { get; set; }

}
