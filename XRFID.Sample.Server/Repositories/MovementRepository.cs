using XRFID.Sample.Server.Database;
using XRFID.Sample.Server.Entities;

namespace XRFID.Sample.Server.Repositories;

public class MovementRepository : BaseRepository<Movement>
{
    public MovementRepository(XRFIDSampleContext context, ILogger<MovementRepository> logger) : base(context, logger)
    {
    }

    public async Task<List<Movement>> DeactivateByReaderId(Guid readerId)
    {
        List<Movement> movments = await GetAsync(g => g.ReaderId == readerId);
        List<Movement> updatedMovments = new List<Movement>();
        foreach (Movement movment in movments)
        {
            movment.IsActive = false;
            updatedMovments.Add(await UpdateAsync(movment));
        }
        return updatedMovments;
    }
}
