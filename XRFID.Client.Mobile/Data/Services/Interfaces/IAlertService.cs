﻿namespace XRFID.Sample.Client.Data.Services.Interfaces;
public interface IAlertService
{
    void ShortToast(string message);

    void LongToast(string message);

}
