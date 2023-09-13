using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System.Text.Json;
using XRFID.Sample.Common.Utils;
using XRFID.Sample.Server.Database;
using XRFID.Sample.Server.Entities;

namespace XRFID.Sample.Server.Repositories.Tests;

[TestFixture()]
public class BaseRepositoryTests
{
    private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions { WriteIndented = true };
    private readonly Reader originalReader;
    private readonly Reader modifiedReader;

    private readonly XRFIDSampleContext _context;
    private readonly BaseRepository<Reader> _readerRepository;
    private readonly UnitOfWork _unitOfWork;

    public BaseRepositoryTests()
    {
        DbContextOptions<XRFIDSampleContext> options = new DbContextOptionsBuilder<XRFIDSampleContext>().UseInMemoryDatabase("testdb").Options;
        _context = new XRFIDSampleContext(options);
        _unitOfWork = new UnitOfWork(_context);
        ILogger<ReaderRepository> logger = new NullLoggerFactory().CreateLogger<ReaderRepository>();
        _readerRepository = new BaseRepository<Reader>(_context, logger);

        originalReader = new Reader
        {
            Id = Guid.Parse("793ebe76-2a1a-4d5d-a1f6-4d921c0c0621"),
            CreatorUserId = "test",
            LastModifierUserId = "test",
            Ip = "127.0.0.1"
        };

        modifiedReader = new Reader
        {
            Id = Guid.Parse("793ebe76-2a1a-4d5d-a1f6-4d921c0c0621"),
            CreatorUserId = "test",
            LastModifierUserId = "test",
            Ip = "127.0.1.1"
        };
    }


    /// <summary>
    /// ensures the Db exists, it may or may not be useless for an InMemory DB
    /// </summary>
    [Test(), Order(0)]
    public void CreateDb()
    {
        _context.Database.EnsureCreated();
        Assert.Pass();
    }

    [Test(), Order(1)]
    public async Task AddAsync()
    {
        Reader reader = await _readerRepository.CreateAsync(originalReader);
        await _unitOfWork.SaveAsync();

        if (!ObjectUtils.GenericEquals(reader, originalReader))
        {
            Assert.Fail();
        }
        Assert.Pass();
    }

    [Test(), Order(2)]
    public async Task UpdateAsync()
    {
        Reader reader = await _readerRepository.UpdateAsync(modifiedReader);
        await _unitOfWork.SaveAsync();
        _context.Entry(reader).State = EntityState.Detached;
        if (!ObjectUtils.GenericEquals(reader, originalReader))
        {
            Assert.Fail();
        }
        Assert.Pass();
    }

    [Test(), Order(3)]
    public async Task DeleteAsync()
    {
        Reader reader = await _readerRepository.DeleteAsync(modifiedReader);
        await _unitOfWork.SaveAsync();
        var result = await _readerRepository.GetAsync(g => g.Id == reader.Id);
        if (result.Any())
        {
            Assert.Fail();
        }
        Assert.Pass();
    }
}