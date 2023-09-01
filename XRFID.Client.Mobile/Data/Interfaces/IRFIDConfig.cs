﻿namespace XRFID.Sample.Client.Data.Interfaces;

public interface IRFIDConfig
{
    Guid Id { get; set; }
    string Name { get; set; }
    bool IsActive { get; set; }
    int Power { get; set; }
    bool DynamicPower { get; set; }
}