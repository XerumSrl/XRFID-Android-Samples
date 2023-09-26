using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using XRFID.Demo.Client.Mobile.Base;
using XRFID.Demo.Client.Mobile.Data.Enums;
using XRFID.Demo.Client.Mobile.Data.Services.Interfaces;
using XRFID.Demo.Client.Mobile.Data.ViewData;
using XRFID.Demo.Client.Mobile.Messages;
using XRFID.Demo.Client.Mobile.Platforms.Android.Interfaces;

namespace XRFID.Demo.Client.Mobile.ViewModels;

[QueryProperty(nameof(TagsView), nameof(TagsView))]
public partial class CheckItemRfidViewModel : BaseRfidViewModel
{
    private readonly IAlertService alertService;

    #region collections
    private Dictionary<String, int> tagDict = new Dictionary<string, int>();

    private ObservableCollection<TagViewData> tagsView = new ObservableCollection<TagViewData>();
    public ObservableCollection<TagViewData> TagsView
    {
        get => tagsView;
        set => SetProperty(ref tagsView, value);
    }
    private ObservableCollection<TagViewData> InitalView { get => tagsView; }
    #endregion

    #region commands
    [RelayCommand]
    public async Task ResetCommand()
    {
        StatsReset();
    }

    [RelayCommand]
    public async Task ConfirmCommand()
    {
        alertService.LongToast("Confirmation command is ok");
    }
    #endregion

    public CheckItemRfidViewModel(IRFIDService rfidService,
        IGeneralSettings generalSettings,
        IAlertService alertService) : base(rfidService, generalSettings)
    {
        this.alertService = alertService;
    }

    #region view methods
    public override Task OnAppearing()
    {
        UpdateIn();
        return base.OnAppearing();
    }
    public override Task OnDisappearing()
    {
        Dispose(); //This dispose method is in base, but due to inherithance implementation details it calls the overridden UpdateOut()
        return base.OnDisappearing();
    }
    #endregion

    #region workflow methods
    public void StatsReset()
    {
        tagDict.Clear();
        foreach (TagViewData tag in TagsView)
        {
            tag.Status = TagViewStatus.Default.ToString();
        }
    }
    #endregion

    #region RFID method
    public override void UpdateIn()
    {
        rfidService.TagRead += TestTagReadEvent;
        rfidService.TriggerEvent += HHTriggerEvent;
        rfidService.ReaderConnectionEvent += ReaderConnectionEvent;

        disposedValue = false;
    }
    public override void UpdateOut()
    {
        rfidService.TagRead -= TestTagReadEvent;
        rfidService.TriggerEvent -= HHTriggerEvent;
        rfidService.ReaderConnectionEvent -= ReaderConnectionEvent;
    }

    private void TestTagReadEvent(IList<ReaderTag> aryTags)
    {
        lock (tagreadlock)
        {
            foreach (ReaderTag tag in aryTags)
            {
                TestUpdateUi(tag.Epc, tag.TagCount, tag.Rssi);
            }
        }
    }

    private void TestUpdateUi(string id, int count, short? rssi)
    {
        if (tagDict.ContainsKey(id))
        {
            tagDict[id] = tagDict[id] + count;
            UpdateCount(id, tagDict[id], rssi);
        }
        else
        {
            tagDict.Add(id, count);
            UpdateList(id, count, rssi);
        }
        totalTagCount += count;
        UpdateCounts();
    }
    #endregion

    #region custom tag methods
    protected override void UpdateList(String tag, int count, short? rssi)
    {
        TagViewData find = TagsView.FirstOrDefault(q => q.Epc == tag);
        if (find is not null)
        {
            find.Status = "FOUND!";
        }
    }

    protected override void UpdateCount(String tag, int count, short? rssi)
    {

    }
    #endregion
}
