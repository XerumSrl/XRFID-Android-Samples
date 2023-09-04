using Com.Zebra.Rfid.Api3;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using XRFID.Sample.Client.Mobile.Data;
using XRFID.Sample.Client.Mobile.Data.Enums;
using XRFID.Sample.Client.Mobile.Data.Services.Interfaces;
using XRFID.Sample.Client.Mobile.Messages;
using XRFID.Sample.Client.Mobile.Platforms.Android.Interfaces;
using Application = Android.App.Application;

namespace XRFID.Sample.Client.Mobile.Platforms.Android.Services;
public class RFIDService : Java.Lang.Object, Readers.IRFIDReaderEventHandler, IRfidEventsListener, IRFIDService
{
    private IGeneralSettings _generalSettings;

    public event TagReadHandler TagRead;
    public event TriggerHandler TriggerEvent;
    public event StatusHandler StatusEvent;
    public event ConnectionHandler ReaderConnectionEvent;
    public event ReaderAppearanceHandler ReaderAppearanceEvent;

    public IList<ReaderDevice> ReadersList = new List<ReaderDevice>();
    public RFIDReader rfidReader;
    private Readers readers;
    ReaderDevice readerDevice;
    private readonly ILogger<RFIDService> logger;

    public bool IsConnected
    {
        get => rfidReader != null && rfidReader.IsConnected;
    }

    private int lastConnectedReaderIndex
    {
        get
        {
            int index = 0;
            try
            {
                index = _generalSettings.ReaderIndex;
            }
            catch (KeyNotFoundException) { }
            return index;
        }
        set
        {
            _generalSettings.ReaderIndex = value;
        }
    }

    public RFIDService(ILogger<RFIDService> logger)
    {
        this.logger = logger;
    }


    public void InitRfidReader(IGeneralSettings generalSettings)
    {
        this._generalSettings = generalSettings;

        Readers.Attach(this);
        Setup();
    }

    #region Inventory

    /// <summary>
    /// Set trigger mode to rfid on resume / screen appearance or start
    /// </summary>
    public void SetTriggerMode()
    {
        //Try to connect
        if (rfidReader != null && !rfidReader.IsConnected)
        {
            ConnectReader(lastConnectedReaderIndex);
        }
        if (IsConnected)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    rfidReader.Config.SetTriggerMode(ENUM_TRIGGER_MODE.RfidMode, true);
                }
                catch (OperationFailureException e)
                {
                    e.PrintStackTrace();
                }
            });
        }
    }

    public bool PerformInventory()
    {
        try
        {
            rfidReader.Actions.Inventory.Perform();
            return true;
        }
        catch (InvalidUsageException e)
        {
            e.PrintStackTrace();
        }
        catch (OperationFailureException e)
        {
            e.PrintStackTrace();
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
        return false;
    }

    public void StopInventory()
    {
        try
        {
            rfidReader.Actions.Inventory.Stop();
        }
        catch (InvalidUsageException e)
        {
            e.PrintStackTrace();
        }
        catch (OperationFailureException e)
        {
            e.PrintStackTrace();
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
    }

    public void Locate(bool start, string tagPattern, string tagMask)
    {
        try
        {
            if (start)
            {
                rfidReader.Actions.TagLocationing.Perform(tagPattern, tagMask, null);
            }
            else
            {
                rfidReader.Actions.TagLocationing.Stop();
            }
        }
        catch (InvalidUsageException e)
        {
            e.PrintStackTrace();
        }
        catch (OperationFailureException e)
        {
            e.PrintStackTrace();
        }
    }

    public void EventReadNotify(RfidReadEvents readEvent)
    {
        TagData[] myTags = rfidReader.Actions.GetReadTags(100);
        List<ReaderTag> readerTagList = new List<ReaderTag>();
        if (myTags != null)
        {
            foreach (TagData tag in myTags)
            {
                readerTagList.Add(new ReaderTag()
                {
                    Epc = tag.TagID,
                    Rssi = tag.PeakRSSI,
                    TagCount = tag.TagSeenCount,
                    //FirstRead = Convert.ToDateTime(tag.SeenTime.UTCTime.FirstSeenTimeStamp.CurrentTime)
                });
            }
            ThreadPool.QueueUserWorkItem(o => TagRead?.Invoke(readerTagList));
        }
    }

    public void EventStatusNotify(RfidStatusEvents rfidStatusEvents)
    {
        if (rfidStatusEvents.StatusEventData.StatusEventType == STATUS_EVENT_TYPE.HandheldTriggerEvent)
        {
            if (rfidStatusEvents.StatusEventData.HandheldTriggerEventData.HandheldEvent == HANDHELD_TRIGGER_EVENT_TYPE.HandheldTriggerPressed)
            {
                ThreadPool.QueueUserWorkItem(o => TriggerEvent?.Invoke(true));
            }
            if (rfidStatusEvents.StatusEventData.HandheldTriggerEventData.HandheldEvent == HANDHELD_TRIGGER_EVENT_TYPE.HandheldTriggerReleased)
            {
                ThreadPool.QueueUserWorkItem(o => TriggerEvent?.Invoke(false));
            }
        }
        else if (rfidStatusEvents.StatusEventData.StatusEventType == STATUS_EVENT_TYPE.InventoryStartEvent)
        {
            ThreadPool.QueueUserWorkItem(o => StatusEvent?.Invoke(rfidStatusEvents.StatusEventData));
        }
        else if (rfidStatusEvents.StatusEventData.StatusEventType == STATUS_EVENT_TYPE.InventoryStopEvent)
        {
            ThreadPool.QueueUserWorkItem(o => StatusEvent?.Invoke(rfidStatusEvents.StatusEventData));
        }
        else if (rfidStatusEvents.StatusEventData.StatusEventType == STATUS_EVENT_TYPE.OperationEndSummaryEvent)
        {
            int rounds = rfidStatusEvents.StatusEventData.OperationEndSummaryData.TotalRounds;
            int totaltags = rfidStatusEvents.StatusEventData.OperationEndSummaryData.TotalTags;
            long timems = rfidStatusEvents.StatusEventData.OperationEndSummaryData.TotalTimeuS / 1000;
            ThreadPool.QueueUserWorkItem(o => StatusEvent?.Invoke(rfidStatusEvents.StatusEventData));
        }
        else if (rfidStatusEvents.StatusEventData.StatusEventType == STATUS_EVENT_TYPE.DisconnectionEvent)
        {
            ReaderConnectionEvent?.Invoke(false);
        }
    }

    #endregion

    #region Reader

    public void Connect()
    {
        rfidReader.Connect();
    }

    /// <summary>
    /// Connnect with reader after instance creation
    /// </summary>
    public void Setup()
    {
        ThreadPool.QueueUserWorkItem(o =>
        {
            GetAvailableReaders();
            ConnectReader(lastConnectedReaderIndex);
        });
    }

    public void RFIDReaderAppeared(ReaderDevice readerDevice)
    {
        ReadersList.Add(readerDevice);
        //ReaderAppearanceEvent?.Invoke(true);
    }

    public void RFIDReaderDisappeared(ReaderDevice readerDevice)
    {
        ReadersList.Remove(readerDevice);
        //ReaderAppearanceEvent?.Invoke(false);
    }

    public IList<ReaderDevice> GetAvailableReaders()
    {
        //bool serialDeviceNotFound = false;
        ReadersList.Clear();

        RfidScannerType scannerType = RfidScannerType.All;

        switch (scannerType)
        {
            // For MC33XX and RFD2000
            case RfidScannerType.Serial:
                try
                {
                    readers = new Readers(Application.Context, ENUM_TRANSPORT.ServiceSerial);
                    ReadersList = readers.AvailableRFIDReaderList;
                }
                catch (Exception)
                {
                    readers.Dispose();
                    readers = null;

                    readers = new Readers(Application.Context, ENUM_TRANSPORT.All);
                    ReadersList = readers.AvailableRFIDReaderList;
                }
                break;

            // For TC26
            case RfidScannerType.Usb:
                try
                {
                    readers = new Readers(Application.Context, ENUM_TRANSPORT.ServiceUsb);
                    ReadersList = readers.AvailableRFIDReaderList;
                }
                catch (Exception)
                {
                    readers.Dispose();
                    readers = null;

                    readers = new Readers(Application.Context, ENUM_TRANSPORT.All);
                    ReadersList = readers.AvailableRFIDReaderList;
                }
                break;

            // For RFD8500
            case RfidScannerType.Bluetooth:
                try
                {
                    readers = new Readers(Application.Context, ENUM_TRANSPORT.Bluetooth);
                    ReadersList = readers.AvailableRFIDReaderList;
                }
                catch (Exception)
                {
                    readers.Dispose();
                    readers = null;

                    readers = new Readers(Application.Context, ENUM_TRANSPORT.All);
                    ReadersList = readers.AvailableRFIDReaderList;
                }
                break;

            default:
                try
                {
                    readers = new Readers(Application.Context, ENUM_TRANSPORT.All);
                    ReadersList = readers.AvailableRFIDReaderList;
                }
                catch (Exception)
                {
                    readers.Dispose();
                    readers = null;

                    readers = new Readers(Application.Context, ENUM_TRANSPORT.All);
                    ReadersList = readers.AvailableRFIDReaderList;
                }
                break;
        }

        // update the already connected reader in list
        if (IsConnected)
        {
            int id = 0;
            for (; id < ReadersList.Count; id++)
            {
                if (ReadersList[id].RFIDReader.HostName.Equals(rfidReader.HostName))
                {
                    break;
                }
            }
            ReadersList[id] = readerDevice;
        }

        return ReadersList;
    }

    public void ConnectReader(int index)
    {
        ThreadPool.QueueUserWorkItem(o =>
        {
            ConnectReaderSync(index);
        });
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void ConnectReaderSync(int index)
    {
        try
        {
            if (ReadersList.Count > 0)
            {
                readerDevice = ReadersList[index];
                rfidReader = readerDevice.RFIDReader;
                Connect();
                ConfigureReader();
                ReaderConnectionEvent?.Invoke(true);
                lastConnectedReaderIndex = index;
            }
        }
        catch (InvalidUsageException e)
        {
            e.PrintStackTrace();
        }
        catch (OperationFailureException e)
        {
            ReaderConnectionEvent?.Invoke(false);
            e.PrintStackTrace();
        }
    }

    /// <summary>
    /// Configure Reader
    /// Setup event listnere and enable required event types
    /// Set trigger mode to rfid
    /// Configure antenna, singulation and trigger setting using single API call
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void ConfigureReader()
    {
        if (rfidReader.IsConnected)
        {
            try
            {
                RfidConfig rfidConfig = _generalSettings?.RfidConfigs.Where(c => c.IsActive).FirstOrDefault();

                //READER EVENTS
                // receive events from reader
                rfidReader.Events.AddEventsListener(this);
                // HH event
                rfidReader.Events.SetHandheldEvent(true);
                // tag event with tag data
                rfidReader.Events.SetTagReadEvent(true);
                rfidReader.Events.SetAttachTagDataWithReadEvent(false);
                //
                rfidReader.Events.SetInventoryStartEvent(true);
                rfidReader.Events.SetInventoryStopEvent(true);
                rfidReader.Events.SetOperationEndSummaryEvent(true);
                rfidReader.Events.SetReaderDisconnectEvent(true);

                //READER CONFIG
                // set trigger mode as rfid, pass second parameter as true so scanner will be disabled
                SetTriggerMode();

                // configure for antenna and singulation etc.
                Antennas.AntennaRfConfig antenna = rfidReader.Config.Antennas.GetAntennaRfConfig(1);
                antenna.SetrfModeTableIndex(0);
                //antenna.TransmitPowerIndex = rfidReader.ReaderCapabilities.GetTransmitPowerLevelValues().Length - 1;

                antenna.TransmitPowerIndex = rfidConfig.Power;
                rfidReader.Config.Antennas.SetAntennaRfConfig(1, antenna);

                //singulation
                Antennas.SingulationControl singulationControl;
                singulationControl = rfidReader.Config.Antennas.GetSingulationControl(1);
                //singulationControl.Session = GetSession(rfidConfig);
                //singulationControl.Action.InventoryState = GetInventoryState(rfidConfig);
                singulationControl.Action.SetPerformStateAwareSingulationAction(false);
                rfidReader.Config.Antennas.SetSingulationControl(1, singulationControl);

                /*
                // set start and stop triggers
                TriggerInfo triggerInfo = new TriggerInfo();
                triggerInfo.StartTrigger.TriggerType = START_TRIGGER_TYPE.StartTriggerTypeImmediate;
                triggerInfo.StopTrigger.TriggerType = STOP_TRIGGER_TYPE.StopTriggerTypeImmediate;

                rfidReader.Config.StartTrigger = triggerInfo.StartTrigger;
                rfidReader.Config.StopTrigger = triggerInfo.StopTrigger;
                */

                TagStorageSettings tagstorage = rfidReader.Config.TagStorageSettings;
                TAG_FIELD[] tag_fields = { TAG_FIELD.PeakRssi, TAG_FIELD.TagSeenCount, TAG_FIELD.AntennaId, TAG_FIELD.FirstSeenTimeStamp };
                tagstorage.SetTagFields(tag_fields);

                // Set default configurations in single API call
                rfidReader.Config.SetDefaultConfigurations(antenna, singulationControl, tagstorage, true, true, null);

                // If RFD8500 then disable batch mode and DPO
                if (rfidReader.ReaderCapabilities.ModelName.Contains("RFD8500"))
                {
                    rfidReader.Config.SetBatchMode(BATCH_MODE.Disable);
                    // Important: DPO should be disabled based on need here disabled for all operations
                    rfidReader.Config.DPOState = DYNAMIC_POWER_OPTIMIZATION.Disable;
                }
            }
            catch (InvalidUsageException e)
            {
                e.PrintStackTrace();
            }
            catch (OperationFailureException e)
            {
                e.PrintStackTrace();
            }
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Disconnect()
    {
        if (rfidReader != null)
        {
            try
            {
                rfidReader.Disconnect();
                ReaderConnectionEvent?.Invoke(false);
            }
            catch (InvalidUsageException e)
            {
                e.PrintStackTrace();
            }
        }
    }

    public void DeInit()
    {
        Readers.Deattach(this);
        readers?.Dispose();
        readers = null;
    }

    #endregion

    private SESSION GetSession(RfidConfig rfidConfig)
    {
        return rfidConfig.Session == Sessions.S0 ? SESSION.SessionS0 : rfidConfig.Session == Sessions.S1 ? SESSION.SessionS1 : rfidConfig.Session == Sessions.S2 ? SESSION.SessionS2 : SESSION.SessionS3;
    }

    private INVENTORY_STATE GetInventoryState(RfidConfig rfidConfig)
    {
        return rfidConfig.InventoryState == InventoryStates.StateA ? INVENTORY_STATE.InventoryStateA : rfidConfig.InventoryState == InventoryStates.StateB ? INVENTORY_STATE.InventoryStateB : INVENTORY_STATE.InventoryStateAbFlip;
    }

}
