using XRFID.Sample.Server.Database;
using XRFID.Sample.Server.Entities;

namespace XRFID.Sample.Server.Repositories;

public class PrinterRepository : BaseRepository<Printer>
{
    public PrinterRepository(XRFIDSampleContext context, ILogger<PrinterRepository> logger) : base(context, logger)
    {
    }
}
