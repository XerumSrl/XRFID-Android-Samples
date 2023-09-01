using XRFID.Sample.Webservice.Database;
using XRFID.Sample.Webservice.Entities;

namespace XRFID.Sample.Webservice.Repositories;

public class ReaderRepository : BaseRepository<Reader>
{
    public ReaderRepository(XRFIDSampleContext context, ILogger<ReaderRepository> logger) : base(context, logger)
    {
    }
}
