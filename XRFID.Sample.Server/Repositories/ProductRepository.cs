using XRFID.Sample.Server.Database;
using XRFID.Sample.Server.Entities;

namespace XRFID.Sample.Server.Repositories;

public class ProductRepository : BaseRepository<Product>
{
    public ProductRepository(XRFIDSampleContext context, ILogger<ProductRepository> logger) : base(context, logger)
    {
    }
}
