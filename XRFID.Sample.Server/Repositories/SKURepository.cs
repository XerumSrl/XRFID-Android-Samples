using XRFID.Sample.Server.Database;
using XRFID.Sample.Server.Entities;

namespace XRFID.Sample.Server.Repositories;

public class SKURepository : BaseRepository<SKU>
{
    public SKURepository(XRFIDSampleContext context, ILogger<SKURepository> logger) : base(context, logger)
    {
    }
}
