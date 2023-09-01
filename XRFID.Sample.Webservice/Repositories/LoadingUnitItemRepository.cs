using XRFID.Sample.Webservice.Database;
using XRFID.Sample.Webservice.Entities;

namespace XRFID.Sample.Webservice.Repositories;

public class LoadingUnitItemRepository : BaseRepository<LoadingUnitItem>
{
    public LoadingUnitItemRepository(XRFIDSampleContext context, ILogger<LoadingUnitItemRepository> logger) : base(context, logger)
    {
    }
}
