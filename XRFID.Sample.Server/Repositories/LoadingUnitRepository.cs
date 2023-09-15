using XRFID.Sample.Server.Database;
using XRFID.Sample.Server.Entities;

namespace XRFID.Sample.Server.Repositories;

public class LoadingUnitRepository : BaseRepository<LoadingUnit>
{
    public LoadingUnitRepository(XRFIDSampleContext context, ILogger<LoadingUnitRepository> logger) : base(context, logger)
    {
    }
}
