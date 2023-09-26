using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;

namespace XRFID.Demo.Server.Repositories;

public class SkuRepository : BaseRepository<Sku>
{
    public SkuRepository(XRFIDSampleContext context, ILogger<SkuRepository> logger) : base(context, logger)
    {
    }
}
