using XRFID.Demo.Client.Mobile.Data.Dto;
using XRFID.Demo.Client.Mobile.Data.Enums;
using XRFID.Demo.Client.Mobile.Data.Services.Interfaces;

namespace XRFID.Demo.Client.Mobile.Data.Services;
public class GeneralSettings : IGeneralSettings
{
    private AppSettings _appSettings = new AppSettings();
    private IServiceHelper _serviceHelper;

    public string ApiServer { get => _appSettings.ApiServer; set => _appSettings.ApiServer = value; }
    public InputTypes InputType { get => _appSettings.InputType; set => _appSettings.InputType = value; }
    public MovementTypes MovementType { get => _appSettings.MovementType; set => _appSettings.MovementType = value; }
    public string MovementListName { get => _appSettings.MovementListName; set => _appSettings.MovementListName = value; }
    public Guid ReaderId { get => _appSettings.ReaderId; set => _appSettings.ReaderId = value; }
    public string DeviceId { get => _appSettings.DeviceId; set => _appSettings.DeviceId = value; }
    public string DeviceIp { get => _appSettings.DeviceIp; set => _appSettings.DeviceIp = value; }
    public string DeviceModel { get => _appSettings.DeviceModel; set => _appSettings.DeviceModel = value; }
    public string DeviceName { get => _appSettings.DeviceName; set => _appSettings.DeviceName = value; }
    public string DeviceVersion { get => _appSettings.DeviceVersion; set => _appSettings.DeviceVersion = value; }
    public string DevicePlatform { get => _appSettings.DevicePlatform; set => _appSettings.DevicePlatform = value; }
    public string DeviceManufacturer { get => _appSettings.DeviceManufacturer; set => _appSettings.DeviceManufacturer = value; }
    public int ReaderIndex { get => _appSettings.ReaderIndex; set => _appSettings.ReaderIndex = value; }
    public List<OrderExtDto> Orders { get; set; }

    public List<RfidConfig> RfidConfigs
    {
        get
        {
            return _appSettings.RfidConfigs;
        }
        set
        {
            _appSettings.RfidConfigs = value;
        }
    }

    public GeneralSettings()
    {
    }

    public void Load(IServiceHelper serviceHelper)
    {
        this._serviceHelper = serviceHelper;
        _appSettings.Load(_serviceHelper);
    }

    public void SaveRfidConfig(RfidConfig rfidConfig)
    {
        _appSettings.Save(rfidConfig);
    }
}
