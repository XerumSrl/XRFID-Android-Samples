using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;

namespace XRFID.Demo.Server.Repositories.Tests;

[TestFixture()]
public class MovementRepositoryTests
{
    private readonly UnitOfWork _unitOfWork;
    private readonly MovementRepository _movementRepository;
    private readonly Reader reader;
    private readonly Movement movement;

    public MovementRepositoryTests()
    {
        DbContextOptions<XRFIDSampleContext> options = new DbContextOptionsBuilder<XRFIDSampleContext>().UseInMemoryDatabase("testdb").Options;
        XRFIDSampleContext context = new XRFIDSampleContext(options);
        _unitOfWork = new UnitOfWork(context);
        ILogger<MovementRepository> logger = new NullLoggerFactory().CreateLogger<MovementRepository>();
        _movementRepository = new MovementRepository(context, logger);

        reader = new Reader
        {
            Id = Guid.Parse("793ebe76-2a1a-4d5d-a1f6-4d921c0c0621"),
            Name = "not_an_FX9600",
            CreatorUserId = "test",
            LastModifierUserId = "test",
        };

        context.Readers.Add(reader);


        movement = new Movement
        {
            Id = reader.Id,
            ReaderId = reader.Id,
            Name = $"{reader.Name}_{DateTime.Now}",
            IsActive = true,
            MovementItems = new List<MovementItem>
            {
                new MovementItem
                {
                    Name= "MI1",
                    IsValid = true
                },
                new MovementItem
                {
                    Name= "MI2",
                    IsValid = true
                }
            }
        };
        context.Movements.Add(movement);
        context.SaveChangesAsync();

    }

    [Test(), Order(1)]
    public async Task DeactivateByReaderIdTest()
    {
        await _movementRepository.DeactivateByReaderId(movement.ReaderId);
        await _unitOfWork.SaveAsync();

        List<Movement> result = await _movementRepository.GetAsync(g => g.ReaderId == movement.ReaderId && g.IsActive);

        Assert.IsFalse(result.Any());
    }

    [Test(), Order(2)]
    public async Task UpdateStatusAsyncTest()
    {
        await _movementRepository.UpdateStatusAsync(movement.Id);
        await _unitOfWork.SaveAsync();
        Movement mov = await _movementRepository.GetAsync(movement.Id) ?? throw new Exception("Movement disappeared from Dataase");

        Assert.IsTrue(mov.IsConsolidated);
    }
}