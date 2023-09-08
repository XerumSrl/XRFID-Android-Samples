using XRFID.Sample.Server.Database;
using XRFID.Sample.Server.Entities;

namespace XRFID.Sample.Server.Repositories;

public class MovementRepository : BaseRepository<Movement>
{
    public MovementRepository(XRFIDSampleContext context, ILogger<MovementRepository> logger) : base(context, logger)
    {
    }
}
