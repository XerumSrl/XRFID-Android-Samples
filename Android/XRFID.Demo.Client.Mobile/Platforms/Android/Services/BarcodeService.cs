using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Symbol.XamarinEMDK;
using Symbol.XamarinEMDK.Barcode;
using XRFID.Demo.Client.Mobile.Messages;
using XRFID.Demo.Client.Mobile.Platforms.Android.Interfaces;

namespace XRFID.Demo.Client.Mobile.Platforms.Android.Services;
public class BarcodeService : IBarcodeService
{
    private EMDKManager emdkMamager;
    private readonly IEMDKService emdkService;
    private readonly ILogger<BarcodeService> logger;
    private BarcodeManager _barcodeManager;
    private Symbol.XamarinEMDK.Barcode.Scanner _barcodeReader;

    private bool IsBarcodeRdrInit = false;
    public bool IsReading { get; protected set; }
    public bool IsEnabled { get => _barcodeReader != null ? _barcodeReader.IsEnabled : false; }

    public BarcodeService(IEMDKService emdkService, ILogger<BarcodeService> logger)
    {
        this.emdkService = emdkService;
        this.logger = logger;
        this._barcodeManager = emdkService.GetBarcodeManager();
        this._barcodeReader = _barcodeManager.GetDevice(BarcodeManager.DeviceIdentifier.Default);
    }

    public void InitBarcodeReader()
    {
        if (!IsBarcodeRdrInit)
        {
            if (emdkService != null)
            {
                try
                {
                    if (_barcodeReader != null)
                    {
                        logger.LogDebug("aggiungo listener Data e Status per il barcode manager");
                        _barcodeReader.Data += Scanner_Data;
                        _barcodeReader.Status += Scanner_Status;
                        _barcodeReader.TriggerType = Symbol.XamarinEMDK.Barcode.Scanner.TriggerTypes.Hard;
                        if (!_barcodeReader.IsEnabled)
                        {
                            EnableBarcodeReader();
                        }

                        IsBarcodeRdrInit = true;
                    }
                    else
                    {
                        try
                        {
                            _barcodeManager = emdkService.GetBarcodeManager();
                            _barcodeReader = _barcodeManager.GetDevice(BarcodeManager.DeviceIdentifier.Default);
                            if (_barcodeReader is null)
                            {
                                throw new Exception();
                            }
                            logger.LogDebug("aggiungo listener Data e Status per il barcode manager");
                            _barcodeReader.Data += Scanner_Data;
                            _barcodeReader.Status += Scanner_Status;
                            _barcodeReader.TriggerType = Symbol.XamarinEMDK.Barcode.Scanner.TriggerTypes.Hard;
                            if (!_barcodeReader.IsEnabled)
                            {
                                EnableBarcodeReader();
                            }

                            IsBarcodeRdrInit = true;
                        }
                        catch (Exception ex)
                        {
                            logger.LogError("Exception: " + ex.Message);
                            logger.LogDebug("Failed to enable scanner.\n");
                        }
                    }
                }
                catch (ScannerException e)
                {
                    logger.LogError("Exception: " + e.Message);
                }
                catch (Exception ex)
                {
                    logger.LogError("Exception: " + ex.Message);
                }
            }
        }
    }

    public void DeinitBarcodeReader()
    {
        if (emdkService != null)
        {
            if (_barcodeReader != null)
            {
                try
                {
                    if (_barcodeReader.IsEnabled)
                    {
                        logger.LogDebug("tolgo listener Data e Status per il barcode reader");
                        _barcodeReader.Data -= Scanner_Data;
                        _barcodeReader.Status -= Scanner_Status;
                        logger.LogDebug("disabilito il barcode reader");
                        _barcodeReader.Disable();
                        IsBarcodeRdrInit = false;
                    }
                }
                catch (ScannerException e)
                {
                    logger.LogDebug("Exception:" + e.Result.Description);
                }
            }

            if (_barcodeManager != null)
            {
                _barcodeReader = null;
                _barcodeManager = null;
            }
            IsBarcodeRdrInit = false;
        }
    }

    public void EnableBarcodeReader()
    {
        if (emdkService != null)
        {
            try
            {
                if (_barcodeReader != null)
                {
                    if (!_barcodeReader.IsEnabled)
                    {
                        logger.LogDebug("abilito il barcode reader");
                        _barcodeReader.Enable();
                        _barcodeReader.Read();
                    }
                }
                else
                {
                    logger.LogDebug("Failed to enable scanner.\n");
                }
            }
            catch (ScannerException e)
            {
                DeinitBarcodeReader();
                InitBarcodeReader();
                logger.LogError("Exception: " + e.Message);
            }
            catch (Exception ex)
            {
                logger.LogError("Exception: " + ex.Message);
            }
        }
    }

    public void DisableBarcodeReader()
    {
        if (emdkService != null)
        {
            if (_barcodeReader != null)
            {
                try
                {
                    if (_barcodeReader.IsEnabled)
                    {
                        logger.LogDebug("disabilito il barcode manager");
                        _barcodeReader.Disable();
                        _barcodeReader.CancelRead();
                    }
                }
                catch (ScannerException e)
                {
                    logger.LogDebug("Exception:" + e.Result.Description);
                }
            }
        }
    }

    private void Scanner_Status(object sender, Symbol.XamarinEMDK.Barcode.Scanner.StatusEventArgs e)
    {
        string statusStr = "";
        StatusData statusData = e.P0;
        StatusData.ScannerStates state = e.P0.State;

        switch (statusData.State.Name())
        {
            case "DISABLED":
                //Scanner is disabled
                statusStr = "Scanner is not enabled";
                logger.LogDebug($"scanner status listener: {statusStr}");
                break;
            case "ENABLED":
                logger.LogDebug($"scanner status listener: onEnabled");
                break;
            case "SCANNING": //Scanner is SCANNING
                statusStr = "Scanner beam is on, aim at the barcode";
                logger.LogDebug($"scanner status listener: {statusStr}");

                break;
            case "IDLE":
                logger.LogDebug("Idle");
                statusStr = "Scanner enabled and idle";
                try
                {
                    if (_barcodeReader.IsEnabled && !_barcodeReader.IsReadPending)
                    {
                        ////TO - DO configurare lo scanner, è possibile solo quando lo scanner è in idle

                        ////  EMDK: Configure the scanner settings
                        //ScannerConfig config = _barcodeReader.GetConfig();
                        //config.SkipOnUnsupported = ScannerConfig.SkipOnUnSupported.None;
                        //config.ReaderParams.ReaderSpecific.ImagerSpecific.AimingPattern = ScannerConfig.AimingPattern.On;
                        //config.ReaderParams.ReaderSpecific.ImagerSpecific.AimType = ScannerConfig.AimType.Trigger;
                        //config.ReaderParams.ReaderSpecific.ImagerSpecific.ScanMode = ScannerConfig.ScanMode.SingleBarcode;
                        //config.ScanParams.DecodeLEDFeedback = true;
                        //config.ReaderParams.ReaderSpecific.ImagerSpecific.PickList = ScannerConfig.PickList.Enabled;
                        //config.DecoderParams.Code39.Enabled = true;
                        //config.DecoderParams.Code128.Enabled = true;
                        //_barcodeReader.SetConfig(config);

                        logger.LogDebug($"scanner status listener: {statusStr}, inizio lettura");
                        _barcodeReader.Read();
                    }
                }
                catch (ScannerException scannEx)
                {
                    logger.LogError("Exception: scanner status listener - ScannerException " + scannEx.Message);
                    statusStr = scannEx.Message;
                }
                break;
            case "ERROR": //Error occurred
                logger.LogError("ERROR: " + statusData.FriendlyName
                        + ": Error-" + statusData.State);
                break;
            case "UNKNOWN":
                logger.LogDebug("scanner status listener: Uknown");
                break;
            default:
                logger.LogDebug("scanner status listener: Default");
                break;

        }
    }

    private void Scanner_Data(object sender, Symbol.XamarinEMDK.Barcode.Scanner.DataEventArgs e)
    {
        ScanDataCollection scanDataCollection = e.P0;

        if (scanDataCollection != null && scanDataCollection.Result == ScannerResults.Success)
        {
            IList<ScanDataCollection.ScanData> scanData = scanDataCollection.GetScanData();
            if (scanData != null && scanData.Count > 0)
            {
                WeakReferenceMessenger.Default.Send(new BarcodeMessage(scanData));
            }
        }
    }
}
