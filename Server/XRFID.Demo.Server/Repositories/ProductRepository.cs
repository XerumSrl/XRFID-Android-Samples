using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;

namespace XRFID.Demo.Server.Repositories;

public class ProductRepository : BaseRepository<Product>
{
    public ProductRepository(XRFIDSampleContext context, ILogger<ProductRepository> logger) : base(context, logger)
    {
    }
}
