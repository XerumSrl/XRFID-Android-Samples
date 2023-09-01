using XRFID.Sample.Webservice.Database;
using XRFID.Sample.Webservice.Entities;

namespace XRFID.Sample.Webservice.Repositories;

public class ProductRepository : BaseRepository<Product>
{
    public ProductRepository(XRFIDSampleContext context, ILogger<ProductRepository> logger) : base(context, logger)
    {
    }
}
