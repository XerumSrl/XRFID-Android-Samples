using XRFID.Sample.Server.Database;
using XRFID.Sample.Server.Entities;

namespace XRFID.Sample.Server.Repositories;

public class LoadingUnitItemRepository : BaseRepository<LoadingUnitItem>
{
    public LoadingUnitItemRepository(XRFIDSampleContext context, ILogger<LoadingUnitItemRepository> logger) : base(context, logger)
    {
    }
}
