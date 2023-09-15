using XRFID.Sample.Server.Database;
using XRFID.Sample.Server.Entities;

namespace XRFID.Sample.Server.Repositories;

public class MovementItemRepository : BaseRepository<MovementItem>
{
    public MovementItemRepository(XRFIDSampleContext context, ILogger<MovementItemRepository> logger) : base(context, logger)
    {
    }
}
