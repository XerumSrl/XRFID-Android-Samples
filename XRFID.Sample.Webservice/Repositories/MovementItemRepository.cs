using XRFID.Sample.Webservice.Database;
using XRFID.Sample.Webservice.Entities;

namespace XRFID.Sample.Webservice.Repositories;

public class MovementItemRepository : BaseRepository<MovementItem>
{
    public MovementItemRepository(XRFIDSampleContext context, ILogger<MovementItemRepository> logger) : base(context, logger)
    {
    }
}
