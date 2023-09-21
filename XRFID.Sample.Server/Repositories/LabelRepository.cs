using XRFID.Sample.Server.Database;
using XRFID.Sample.Server.Entities;

namespace XRFID.Sample.Server.Repositories;

public class LabelRepository : BaseRepository<Label>
{
    public LabelRepository(XRFIDSampleContext context, ILogger<LabelRepository> logger) : base(context, logger)
    {
    }
}
