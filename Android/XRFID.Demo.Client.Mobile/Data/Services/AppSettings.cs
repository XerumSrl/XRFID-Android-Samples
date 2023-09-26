using Plugin.DeviceInfo;
using System.Text.Json;
using XRFID.Demo.Client.Mobile.Data.Enums;
using XRFID.Demo.Client.Mobile.Data.Services.Interfaces;

namespace XRFID.Demo.Client.Mobile.Data.Services;
public class AppSettings : IAppSettings
{

    #region Setting Constants

    private const string SettingsKey = "settings_key";
    private static readonly string SettingsDefault = string.Empty;

    #endregion

    public string ApiServer
    {
        get
        {
            return Preferences.Default.Get(nameof(ApiServer), "https://192.168.0.18/api/");
        }
        set
        {
            Preferences.Default.Set(nameof(ApiServer), value);
        }
    }

    public InputTypes InputType
    {
        get
        {
            return (InputTypes)Preferences.Default.Get(nameof(InputType), (int)InputTypes.Rfid);
        }
        set
        {
            Preferences.Default.Set(nameof(InputType), (int)value);
        }
    }

    public MovementTypes MovementType
    {
        get
        {
            return (MovementTypes)Preferences.Default.Get(nameof(InputType), (int)MovementTypes.Inventory);
        }
        set
        {
            Preferences.Default.Set(nameof(MovementType), (int)value);
        }
    }

    public string MovementListName
    {
        get
        {
            return Preferences.Default.Get(nameof(MovementListName), "Lista di carico 0");
        }
        set
        {
            Preferences.Default.Set(nameof(MovementListName), value);
        }
    }

    public Guid ReaderId
    {
        get
        {
            return Preferences.Default.Get(nameof(ReaderId), Guid.Empty);
        }
        set
        {
            Preferences.Default.Set(nameof(ReaderId), value);
        }
    }

    public string DeviceId
    {
        get
        {
            return Preferences.Default.Get(nameof(DeviceId), String.Empty);
        }
        set
        {
            Preferences.Default.Set(nameof(DeviceId), value);
        }
    }



    public string DeviceIp
    {
        get
        {
            return Preferences.Default.Get(nameof(DeviceIp), String.Empty);
        }
        set
        {
            Preferences.Default.Set(nameof(DeviceIp), value);
        }
    }

    public string DeviceModel
    {
        get
        {
            return Preferences.Default.Get(nameof(DeviceModel), String.Empty);
        }
        set
        {
            Preferences.Default.Set(nameof(DeviceModel), value);
        }
    }

    public string DeviceName
    {
        get
        {
            return Preferences.Default.Get(nameof(DeviceName), String.Empty);
        }
        set
        {
            Preferences.Default.Set(nameof(DeviceName), value);
        }
    }

    public string DeviceVersion
    {
        get
        {
            return Preferences.Default.Get(nameof(DeviceVersion), String.Empty);
        }
        set
        {
            Preferences.Default.Set(nameof(DeviceVersion), value);
        }
    }

    public string DevicePlatform
    {
        get
        {
            return Preferences.Default.Get(nameof(DevicePlatform), String.Empty);
        }
        set
        {
            Preferences.Default.Set(nameof(DevicePlatform), value);
        }
    }

    public string DeviceManufacturer
    {
        get
        {
            return Preferences.Default.Get(nameof(DeviceManufacturer), String.Empty);
        }
        set
        {
            Preferences.Default.Set(nameof(DeviceManufacturer), value);
        }
    }

    public int ReaderIndex
    {
        get
        {
            return Preferences.Default.Get(nameof(ReaderIndex), 0);
        }
        set
        {
            Preferences.Default.Set(nameof(ReaderIndex), value);
        }
    }

    private string RfidConfigsStr
    {
        get
        {
            return Preferences.Default.Get(nameof(RfidConfigsStr), JsonSerializer.Serialize(DefaultRfidConfigs()));
        }
        set
        {
            Preferences.Default.Set(nameof(RfidConfigsStr), value);
        }
    }

    public List<RfidConfig> RfidConfigs { get; set; }

    public AppSettings()
    {

    }

    public void Load(IServiceHelper serviceHelper)
    {
        DeviceId = CrossDeviceInfo.Current.Id;
        DeviceIp = serviceHelper.GetDeviceIp();
        DeviceManufacturer = CrossDeviceInfo.Current.Manufacturer;
        DeviceModel = CrossDeviceInfo.Current.Model;
        DeviceName = CrossDeviceInfo.Current.DeviceName;
        DevicePlatform = CrossDeviceInfo.Current.Platform.ToString();
        DeviceVersion = CrossDeviceInfo.Current.Version;
        RfidConfigs = new List<RfidConfig>();

        RfidConfigs = JsonSerializer.Deserialize<List<RfidConfig>>(RfidConfigsStr);
    }

    public void Save(RfidConfig rfidConfig)
    {
        RfidConfig currRfidConfig = RfidConfigs.Where(c => c.Id == rfidConfig.Id).FirstOrDefault();
        currRfidConfig.DynamicPower = rfidConfig.DynamicPower;
        currRfidConfig.InventoryState = rfidConfig.InventoryState;
        currRfidConfig.IsActive = rfidConfig.IsActive;
        currRfidConfig.Name = rfidConfig.Name;
        currRfidConfig.Power = rfidConfig.Power;
        currRfidConfig.Session = rfidConfig.Session;

        if (rfidConfig.IsActive)
        {
            RfidConfigs.Where(c => c.Id != rfidConfig.Id).ToList().ForEach(c => c.IsActive = false);
        }

        if (RfidConfigs.Count(c => c.IsActive) == 0)
        {
            RfidConfigs.Where(c => c.Name == "Default Profile").FirstOrDefault().IsActive = true;
        }

        RfidConfigsStr = JsonSerializer.Serialize(RfidConfigs);
    }

    private List<RfidConfig> DefaultRfidConfigs()
    {
        return new List<RfidConfig>()
            {
                new RfidConfig() { Id = Guid.NewGuid(), Name = "Default Profile", IsActive = true, Power = 300, DynamicPower = false, InventoryState = InventoryStates.StateA, Session = Sessions.S0 },
                new RfidConfig() { Id = Guid.NewGuid(), Name = "Default Profile 2", IsActive = false, Power = 800, DynamicPower = true, InventoryState = InventoryStates.FlipAB, Session = Sessions.S1 },
            };
    }
}
