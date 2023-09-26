using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;

namespace XRFID.Demo.Server.Repositories;

public class PrinterRepository : BaseRepository<Printer>
{
    public PrinterRepository(XRFIDSampleContext context, ILogger<PrinterRepository> logger) : base(context, logger)
    {
    }
}
