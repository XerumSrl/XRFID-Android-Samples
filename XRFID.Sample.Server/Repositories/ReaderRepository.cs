using XRFID.Sample.Server.Database;
using XRFID.Sample.Server.Entities;

namespace XRFID.Sample.Server.Repositories;

public class ReaderRepository : BaseRepository<Reader>
{
    public ReaderRepository(XRFIDSampleContext context, ILogger<ReaderRepository> logger) : base(context, logger)
    {
    }
}
