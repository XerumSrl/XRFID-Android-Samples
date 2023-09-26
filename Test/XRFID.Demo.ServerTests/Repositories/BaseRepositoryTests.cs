using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System.Text.Json;
using XRFID.Demo.Common.Utils;
using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;

namespace XRFID.Demo.Server.Repositories.Tests;

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
        Assert.IsTrue(ObjectUtils.GenericEquals(reader, originalReader));
    }

    [Test(), Order(2)]
    public async Task UpdateAsync()
    {
        Reader reader = await _readerRepository.UpdateAsync(modifiedReader);
        await _unitOfWork.SaveAsync();
        Assert.IsTrue(ObjectUtils.GenericEquals(reader, modifiedReader));
    }

    [Test(), Order(3)]
    public async Task DeleteAsync()
    {
        Reader reader = await _readerRepository.DeleteAsync(modifiedReader.Id);
        await _unitOfWork.SaveAsync();
        Reader? result = await _readerRepository.GetAsync(reader.Id);
        Assert.IsNull(result);
    }

    [Test, Order(4)]
    public async Task CombinedAddUpdateAsync()
    {
        Reader reader = await _readerRepository.CreateAsync(originalReader);
        await _unitOfWork.SaveAsync();
        reader = await _readerRepository.UpdateAsync(modifiedReader);
        await _unitOfWork.SaveAsync();
        reader = await _readerRepository.DeleteAsync(reader.Id);
        await _unitOfWork.SaveAsync();
        var result = await _readerRepository.GetAsync(g => g.Id == reader.Id);

        Assert.IsFalse(result.Any());
    }
}