using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;

namespace XRFID.Demo.Server.Repositories;

public class ReaderRepository : BaseRepository<Reader>
{
    public ReaderRepository(XRFIDSampleContext context, ILogger<ReaderRepository> logger) : base(context, logger)
    {
    }
}
