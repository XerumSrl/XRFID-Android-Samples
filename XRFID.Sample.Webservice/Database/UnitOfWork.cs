namespace XRFID.Sample.Webservice.Database;

public class UnitOfWork : IDisposable
{
    private XRFIDSampleContext _context;

    private bool isDisposed = false;

    public UnitOfWork(XRFIDSampleContext context)
    {
        _context = context;
    }

    public void Save()
    {
        _context.SaveChanges();
    }
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    private void Dispose(bool disposing)
    {
        if (!isDisposed && disposing)
        {
            _context.Dispose();
        }

        isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
