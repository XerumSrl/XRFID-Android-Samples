﻿using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace XRFID.Sample.Client.Mobile.Data.ViewData;
public class OrderViewData : ObservableObject, INotifyPropertyChanged
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
