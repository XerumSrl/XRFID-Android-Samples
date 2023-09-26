using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;

namespace XRFID.Demo.Server.Repositories;

public class LoadingUnitRepository : BaseRepository<LoadingUnit>
{
    public LoadingUnitRepository(XRFIDSampleContext context, ILogger<LoadingUnitRepository> logger) : base(context, logger)
    {
    }
}
