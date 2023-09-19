using Microsoft.IdentityModel.Tokens;
using Xerum.XFramework.Common.Enums;
using XRFID.Sample.Pages.Data;
using XRFID.Sample.Pages.Data.Enums;
using XRFID.Sample.Server.Entities;
using XRFID.Sample.Server.Repositories;

namespace XRFID.Sample.Server.Workers;

public class CheckPageWorker
{
    private readonly MovementItemRepository movementItemRepository;
    private readonly ILogger<CheckPageWorker> logger;
    private List<ViewItem> viewItems = new List<ViewItem>();

    public CheckPageWorker(MovementItemRepository movementItemRepository, ILogger<CheckPageWorker> logger)
    {
        this.movementItemRepository = movementItemRepository;
        this.logger = logger;
    }

    public async Task SetViewItems()
    {
        viewItems.Clear();

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
                    ViewItem vItem = new ViewItem
                    {
                        Name = dItem.Name ?? string.Empty,
                        Epc = dItem.Epc,
                        Description = dItem.Description,
                        CheckStatus = dItem.Status == ItemStatus.Found ? CheckStatusEnum.Found :
                                             (dItem.Status == ItemStatus.NotFound ? CheckStatusEnum.NotFound : CheckStatusEnum.Error),
                    };
                    viewItems.Add(vItem);
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
}
