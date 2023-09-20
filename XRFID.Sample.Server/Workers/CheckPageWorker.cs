using Microsoft.IdentityModel.Tokens;
using Xerum.XFramework.Common.Enums;
using XRFID.Sample.Server.Entities;
using XRFID.Sample.Server.Repositories;
using XRFID.Sample.Server.ViewModels;
using XRFID.Sample.Server.ViewModels.Enums;

namespace XRFID.Sample.Server.Workers;

public class CheckPageWorker
{
    private readonly MovementItemRepository movementItemRepository;
    private readonly ILogger<CheckPageWorker> logger;
    private List<CheckItemModel> _viewItems = new List<CheckItemModel>();

    public CheckPageWorker(IServiceProvider serviceProvider, ILogger<CheckPageWorker> logger)
    {
        var scope = serviceProvider.CreateScope();
        movementItemRepository = scope.ServiceProvider.GetRequiredService<MovementItemRepository>();

        this.logger = logger;
    }

    public async Task SetViewItems()
    {
        _viewItems.Clear();

        List<MovementItem> dailyItems = new List<MovementItem>();

        try
        {
            dailyItems = await movementItemRepository.GetAsync(q => q.CreationTime >= DateTime.Today);
            if (dailyItems.IsNullOrEmpty())
            {
                return;
            }

            foreach (var dItem in dailyItems)
            {
                try
                {
                    CheckItemModel vItem = new CheckItemModel
                    {
                        Name = dItem.Name ?? string.Empty,
                        Epc = dItem.Epc,
                        Description = dItem.Description,
                        CheckStatus = dItem.Status == ItemStatus.Found ? CheckStatusEnum.Found :
                                             (dItem.Status == ItemStatus.NotFound ? CheckStatusEnum.NotFound : CheckStatusEnum.Error),
                        DateTime = dItem.LastModificationTime,
                    };
                    _viewItems.Add(vItem);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "");
                }
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    public async Task<List<CheckItemModel>> GetViewItems()
    {
        List<CheckItemModel> items = new List<CheckItemModel>();
        items = _viewItems.OrderByDescending(o => o.DateTime).ToList();

        return items;
    }

    public bool ItemsIsEmpty()
    {
        return !_viewItems.Any();
    }
}
