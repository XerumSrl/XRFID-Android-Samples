﻿using XRFID.Sample.Client.Mobile.Data.Enums;
using XRFID.Sample.Client.Mobile.Data.Interfaces;

namespace XRFID.Sample.Client.Mobile.Data;
public class RfidConfig : IRFIDConfig
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public int Power { get; set; }
    public Sessions Session { get; set; }
    public InventoryStates InventoryState { get; set; }
    public bool DynamicPower { get; set; }
}
