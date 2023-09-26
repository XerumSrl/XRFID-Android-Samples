using AutoMapper;
using MassTransit;
using Xerum.XFramework.Common.Enums;
using XRFID.Demo.Server.Contracts;
using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;
using XRFID.Demo.Server.Repositories;
using XRFID.Demo.Server.StateMachines.Shipment.Interfaces;
using XRFID.Demo.Server.StateMachines.Shipment.States;

namespace XRFID.Demo.Server.StateMachines.Shipment.Actions;
internal class InitializeCreateMovementActivity :
    IStateMachineActivity<ShipmentState, IGpiEvent>
{
    private readonly IServiceProvider serviceProvider;
    private readonly ReaderRepository readerRepository;
    private readonly MovementRepository movementRepository;
    private readonly MovementItemRepository movementItemRepository;
    private readonly LoadingUnitRepository loadingUnitRepository;
    private readonly LoadingUnitItemRepository loadingUnitItemRepository;
    private readonly UnitOfWork uowk;
    private readonly IMapper mapper;
    private readonly ILogger<InitializeCreateMovementActivity> logger;

    public InitializeCreateMovementActivity(IServiceProvider serviceProvider,
                                            ReaderRepository readerRepository,
                                            MovementRepository movementRepository,
                                            MovementItemRepository movementItemRepository,
                                            LoadingUnitRepository loadingUnitRepository,
                                            LoadingUnitItemRepository loadingUnitItemRepository,
                                            UnitOfWork uowk,
                                            IMapper mapper,
                                            ILogger<InitializeCreateMovementActivity> logger)
    {
        this.serviceProvider = serviceProvider;
        this.readerRepository = readerRepository;
        this.movementRepository = movementRepository;
        this.movementItemRepository = movementItemRepository;
        this.loadingUnitRepository = loadingUnitRepository;
        this.loadingUnitItemRepository = loadingUnitItemRepository;
        this.uowk = uowk;
        this.mapper = mapper;
        this.logger = logger;
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<ShipmentState, IGpiEvent> context, IBehavior<ShipmentState, IGpiEvent> next)
    {
        Reader? reader = (await readerRepository.GetAsync(q => q.Id == context.Saga.ReaderId)).FirstOrDefault();
        if (reader is null || context.Saga.ReaderId == Guid.Empty)
        {
            logger.LogWarning("CreateMovementActivity|Unable to initialize shipment state machine. Reader id is empty");
            throw new InvalidOperationException("CreateMovementActivity|Reader id is empty");
        }

        //Creo la movement dalla loading unit
        LoadingUnit? loadingUnit = (await loadingUnitRepository.GetAsync(q => q.ReaderId == reader.Id && q.IsActive))
                                                      .OrderByDescending(o => o.CreationTime)
                                                      .FirstOrDefault();
        if (loadingUnit is null)
        {
            logger.LogDebug("CreateMovementActivity|No LoadingUnitItems available");

            loadingUnit = await loadingUnitRepository.CreateAsync(new LoadingUnit
            {
                Id = Guid.NewGuid(),
                Description = $"Shipment {DateTime.Now}",
                Name = $"{reader.Name}_{DateTime.Now}",
                ReaderId = context.Message.ReaderId,
                IsActive = true,
                IsConsolidated = false,
            });


            await movementRepository.DeactivateByReaderId(reader.Id);
            if (context.Message.GpiId == 2)
            {

            }
            var newMovement = new Movement
            {
                ReaderId = reader.Id,
                Name = $"{reader.Name}_{DateTime.Now}",
                IsActive = true,

                Direction = context.Message.GpiId == 2 ? Common.Enumerations.MovementDirection.In :
                (context.Message.GpiId == 3 ? Common.Enumerations.MovementDirection.Out : Common.Enumerations.MovementDirection.In),
            };
            await movementRepository.CreateAsync(newMovement);
            await uowk.SaveAsync();
        }
        else
        {
            List<LoadingUnitItem> loadingUnitItems = await loadingUnitItemRepository.GetAsync(q => q.LoadingUnitId == loadingUnit.Id);
            if (loadingUnitItems is null)
            {
                logger.LogDebug("CreateMovementActivity|No LoadingUnitItems available");
            }


            await movementRepository.DeactivateByReaderId(reader.Id);

            var newMovement = new Movement
            {
                ReaderId = reader.Id,
                Name = $"{reader.Name}_{DateTime.Now}",
                IsActive = true,

                Direction = context.Message.GpiId == 2 ? Common.Enumerations.MovementDirection.In :
                (context.Message.GpiId == 3 ? Common.Enumerations.MovementDirection.Out : Common.Enumerations.MovementDirection.In),

                MovementItems = new List<MovementItem>(),
            };

            if (loadingUnitItems is not null)
            {
                foreach (var loadingUnitItem in loadingUnitItems)
                {
                    if (loadingUnitItem.Status == ItemStatus.NotFound ||
                        loadingUnitItem.Status == ItemStatus.Found)
                    {
                        MovementItem movementItem = new MovementItem
                        {
                            Name = loadingUnitItem.Name,
                            Code = loadingUnitItem.Code,
                            Reference = loadingUnitItem.Reference,

                            Description = loadingUnitItem.Epc,
                            SerialNumber = loadingUnitItem.SerialNumber ?? String.Empty,
                            Epc = loadingUnitItem.Epc,

                            IsValid = true,

                            LoadingUnitItemId = loadingUnitItem.Id,

                            MovementId = newMovement.Id,
                        };

                        newMovement.MovementItems.Add(movementItem);
                    }

                }
            }
            await movementRepository.CreateAsync(newMovement);
            await uowk.SaveAsync();
        }

        var movement = (await movementRepository.GetAsync(q => q.ReaderId == reader.Id && q.IsActive)).FirstOrDefault();
        if (movement is null)
        {
            logger.LogWarning("CreateMovementActivity|Missing movement for reader {Id}", reader.Id);
            throw new InvalidOperationException($"CreateMovementActivity|Missing movement for reader {reader.Id}");
        }

        // faccio coincidere le due informazioni
        context.Saga.MovementId = movement.Id;

        //Send message to refresh UI
        await context.Publish(new StateMachineUiTagPublish
        {
            ReaderId = context.Message.ReaderId,
            ActivMoveId = movement.Id,
        });

        #region Update Reader
        if (reader.ActiveMovementId != context.Saga.MovementId)
        {
            logger.LogDebug("StartActivity|Updating reader {Id}: CorrelationId: {CorrelationId} -> {CorrelationId} MovementId: {MovementId} -> {MovementId}",
                reader.Id, reader.CorrelationId, context.Saga.CorrelationId, reader.ActiveMovementId, context.Saga.MovementId);

            reader.ActiveMovementId = context.Saga.MovementId;

            await readerRepository.UpdateAsync(reader);
            await uowk.SaveAsync();
        }
        #endregion

        await next.Execute(context);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<ShipmentState, IGpiEvent, TException> context, IBehavior<ShipmentState, IGpiEvent> next) where TException : Exception
    {
        return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("createMovement");
    }
}
