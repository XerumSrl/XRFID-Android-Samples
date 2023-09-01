﻿using XRFID.Sample.Client.Data.Dto;
using XRFID.Sample.Client.Data.Enums;

namespace XRFID.Sample.Client.Data.Services.Interfaces;

public interface IGeneralSettings
{
    string ApiServer { get; set; }
    InputTypes InputType { get; set; }
    MovementTypes MovementType { get; set; }
    string MovementListName { get; set; }
    Guid ReaderId { get; set; }
    string DeviceId { get; set; }
    string DeviceIp { get; set; }
    string DeviceModel { get; set; }
    string DeviceName { get; set; }
    string DeviceVersion { get; set; }
    string DevicePlatform { get; set; }
    string DeviceManufacturer { get; set; }
    int ReaderIndex { get; set; }
    List<RfidConfig> RfidConfigs { get; set; }
    List<OrderExtDto> Orders { get; set; }

    void Load(IServiceHelper serviceHelper);

    void SaveRfidConfig(RfidConfig rfidConfig);
}