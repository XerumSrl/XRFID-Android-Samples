using Com.Zebra.Rfid.Api3;
using XRFID.Sample.Client.Data.Services.Interfaces;
using XRFID.Sample.Client.Messages;

namespace XRFID.Sample.Client.Platforms.Android.Interfaces;

public delegate void TagReadHandler(IList<ReaderTag> tags);
public delegate void TriggerHandler(bool pressed);
public delegate void StatusHandler(Events.StatusEventData statusEvent);
public delegate void ConnectionHandler(bool connected);
public delegate void ReaderAppearanceHandler(bool appeared);
public interface IRFIDService
{
    event TagReadHandler TagRead;
    event TriggerHandler TriggerEvent;
    event StatusHandler StatusEvent;
    event ConnectionHandler ReaderConnectionEvent;
    event ReaderAppearanceHandler ReaderAppearanceEvent;

    bool IsConnected { get; }

    void InitRfidReader(IGeneralSettings generalSettings);

    void ConfigureReader();

    void Disconnect();

    void Connect();

    void DeInit();

    bool PerformInventory();

    void StopInventory();

    void Locate(bool start, string tagPattern, string tagMask);

    void SetTriggerMode();
}