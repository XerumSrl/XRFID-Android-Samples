using XRFID.Sample.Webservice.Database;
using XRFID.Sample.Webservice.Entities;

namespace XRFID.Sample.Webservice.Repositories;

public class MovementRepository : BaseRepository<Movement>
{
    public MovementRepository(XRFIDSampleContext context, ILogger<MovementRepository> logger) : base(context, logger)
    {
    }
}
