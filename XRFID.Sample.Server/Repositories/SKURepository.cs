using XRFID.Sample.Server.Database;
using XRFID.Sample.Server.Entities;

namespace XRFID.Sample.Server.Repositories;

public class SkuRepository : BaseRepository<Sku>
{
    public SkuRepository(XRFIDSampleContext context, ILogger<SkuRepository> logger) : base(context, logger)
    {
    }
}
