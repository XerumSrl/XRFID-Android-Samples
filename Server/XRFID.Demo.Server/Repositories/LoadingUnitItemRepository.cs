using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;

namespace XRFID.Demo.Server.Repositories;

public class LoadingUnitItemRepository : BaseRepository<LoadingUnitItem>
{
    public LoadingUnitItemRepository(XRFIDSampleContext context, ILogger<LoadingUnitItemRepository> logger) : base(context, logger)
    {
    }
}
