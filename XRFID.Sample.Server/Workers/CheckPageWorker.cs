﻿using Microsoft.IdentityModel.Tokens;
using Xerum.XFramework.Common.Enums;
using XRFID.Sample.Server.Entities;
using XRFID.Sample.Server.Repositories;
using XRFID.Sample.Server.ViewModels;
using XRFID.Sample.Server.ViewModels.Enums;

namespace XRFID.Sample.Server.Workers;

public class CheckPageWorker
{
    private readonly MovementItemRepository movementItemRepository;
    private readonly MovementRepository movementRepository;
    private readonly ILogger<CheckPageWorker> logger;


    private List<CheckItemModel> _viewItems = new List<CheckItemModel>();
    private Guid ActiveId = Guid.Empty;

    private Movement _movement;

    public bool Gpi1IsOn = false;
    public bool Gpi2IsOn = false;
    public bool Gpi3IsOn = false;

    public CheckPageWorker(IServiceProvider serviceProvider, ILogger<CheckPageWorker> logger)
    {
        var scope = serviceProvider.CreateScope();
        movementItemRepository = scope.ServiceProvider.GetRequiredService<MovementItemRepository>();
        movementRepository = scope.ServiceProvider.GetRequiredService<MovementRepository>();

        this.logger = logger;
    }

    public async Task SetViewItem(string epc)
    {
        CheckItemModel? foundItem = _viewItems.Where(w => w.Epc == epc).FirstOrDefault();
        if (foundItem is not null)
        {
            if (foundItem.CheckStatus == CheckStatusEnum.NotFound)
            {
                foundItem.CheckStatus = CheckStatusEnum.Found;
            }

            foundItem.Direction = _movement?.Direction ?? Common.Enumerations.MovementDirection.In;
        }
        else
        {
            var item = (await movementItemRepository.GetAsync(q => q.Epc == epc)).FirstOrDefault();
            if (item is null)
            {
                return;
            }

            _viewItems.Add(new CheckItemModel
            {
                Name = item.Name ?? string.Empty,
                Epc = epc,
                Description = item.Description,
                CheckStatus = item.Status == ItemStatus.Found ? CheckStatusEnum.Found :
                                         (item.Status == ItemStatus.NotFound ? CheckStatusEnum.NotFound : CheckStatusEnum.Error),
                DateTime = item.LastModificationTime,
            });
        }
    }

    public async Task<List<CheckItemModel>> GetViewItems()
    {
        List<CheckItemModel> items = new List<CheckItemModel>();
        items = _viewItems.OrderByDescending(o => o.DateTime).ToList();

        return items;
    }

    public async Task<bool> IdIsEqual(Guid Id)
    {
        if (ActiveId == Id)
        {
            return true;
        }

        ActiveId = Id;
        _viewItems.Clear();

        List<MovementItem> itemList = await movementItemRepository.GetAsync(q => q.MovementId == Id);
        if (itemList.IsNullOrEmpty())
        {
            throw new Exception("No items");
        }

        _movement = await movementRepository.GetAsync(Id);

        foreach (var item in itemList)
        {
            try
            {
                CheckItemModel vItem = new CheckItemModel
                {
                    Name = item.Name ?? string.Empty,
                    Epc = item.Epc,
                    Description = item.Description,
                    CheckStatus = item.Status == ItemStatus.Found ? CheckStatusEnum.Found :
                                         (item.Status == ItemStatus.NotFound ? CheckStatusEnum.NotFound : CheckStatusEnum.Error),
                    DateTime = item.LastModificationTime,
                };
                _viewItems.Add(vItem);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "");
            }
        }

        return false;
    }

    public async Task<bool> StartStop(int pin)
    {
        switch (pin)
        {
            case 1:
                Gpi1IsOn = !Gpi1IsOn;
                return Gpi1IsOn;
            case 2:
                Gpi2IsOn = !Gpi2IsOn;
                return Gpi2IsOn;
            case 3:
                Gpi3IsOn = !Gpi3IsOn;
                return Gpi3IsOn;
            default:
                Gpi1IsOn = false;
                Gpi2IsOn = false;
                Gpi3IsOn = false;
                return false;
        }
    }

    public bool ItemsIsEmpty()
    {
        return !_viewItems.Any();
    }
}
