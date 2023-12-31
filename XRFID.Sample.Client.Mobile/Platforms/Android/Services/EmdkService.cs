﻿using AndroidX.Fragment.App;
using Symbol.XamarinEMDK.Barcode;
using XRFID.Sample.Client.Mobile.Platforms.Android.Interfaces;

namespace XRFID.Sample.Client.Mobile.Platforms.Android.Services;
public class EmdkService : Fragment, IEMDKService
{
    private BarcodeManager barcodeManager;
    public EmdkService()
    {

    }

    public BarcodeManager GetBarcodeManager()
    {
        return barcodeManager;
    }

    public void SetBarcodeManager(BarcodeManager barcodeManager)
    {
        this.barcodeManager = barcodeManager;
    }
}

