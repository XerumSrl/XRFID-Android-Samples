using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;

namespace XRFID.Demo.Server.Repositories;

public class LabelRepository : BaseRepository<Label>
{
    public LabelRepository(XRFIDSampleContext context, ILogger<LabelRepository> logger) : base(context, logger)
    {
    }
}
