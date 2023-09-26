using XRFID.Demo.Client.Mobile.Data.Enums;

namespace XRFID.Demo.Client.Mobile.Data.Services.Interfaces;

public interface IAppSettings : ISetting
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

    void Load(IServiceHelper serviceHelper);

    void Save(RfidConfig rfidConfig);
}