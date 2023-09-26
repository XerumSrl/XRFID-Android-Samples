using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;

namespace XRFID.Demo.Server.Repositories;

public class MovementItemRepository : BaseRepository<MovementItem>
{
    public MovementItemRepository(XRFIDSampleContext context, ILogger<MovementItemRepository> logger) : base(context, logger)
    {
    }
}
