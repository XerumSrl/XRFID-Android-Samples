using XRFID.Sample.Webservice.Database;
using XRFID.Sample.Webservice.Entities;

namespace XRFID.Sample.Webservice.Repositories;

public class LoadingUnitRepository : BaseRepository<LoadingUnit>
{
    public LoadingUnitRepository(XRFIDSampleContext context, ILogger<LoadingUnitRepository> logger) : base(context, logger)
    {
    }
}
