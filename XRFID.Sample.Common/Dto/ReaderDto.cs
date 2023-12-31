﻿using Xerum.XFramework.Common.Dto;
using Xerum.XFramework.Common.Enums;

namespace XRFID.Sample.Common.Dto;

public class ReaderDto : RestEntityDto
{
    public Guid? ActiveMovementId { get; set; }

    public string? Uid { get; set; }

    public string? MacAddress { get; set; }

    public string? Model { get; set; }

    public string? Version { get; set; }

    public string? SerialNumber { get; set; }

    public string? ReaderOS { get; set; }

    public string Ip { get; set; } = "127.0.0.1";

    public ReaderStatus Status { get; set; } = ReaderStatus.Disconnected;
}
