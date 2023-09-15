using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Xerum.XFramework.Common;
using Xerum.XFramework.Common.Enums;
using Xerum.XFramework.Common.Exceptions;
using Xerum.XRFID.Common.Dto;
using Xerum.XRFID.Common.MassTransit.Contracts.StateMachine;
using XRFID.Server.Application.Managers.Interfaces;
using XRFID.Server.Application.Publishers.Interfaces;
using XRFID.Server.Application.Services;
using XRFID.Server.Application.Utilities.Interfaces;
using XRFID.Server.Modules.Hubs;
using XRFID.Server.StateMachines.Shipment.Contracts;
using XRFID.Server.StateMachines.Shipment.Interfaces;

namespace XRFID.Sample.StateMachines.Shipment.Consumers;

public class ShipmentTagConsumer :
    IConsumer<ShipmentTagData>
{
    private readonly ILogger<ShipmentTagConsumer> logger;
    private readonly IReaderManager readerManager;
    private readonly IReaderService readerService;
    private readonly IMovementManager movementManager;
    private readonly IMovementItemManager movementItemManager;
    private readonly ILoadingUnitManager loadingUnitManager;
    private readonly ILoadingUnitItemManager loadingUnitItemManager;
    private readonly IGpoUtility gpoUtility;
    private readonly IHubContext<TagHub> tagHub;
    private readonly IShipmentPublisher shipmentPublisher;

    public ShipmentTagConsumer(ILogger<ShipmentTagConsumer> logger,
        IReaderManager readerManager,
        IReaderService readerService,
        IMovementManager movementManager,
        IMovementItemManager movementItemManager,
        ILoadingUnitManager loadingUnitManager,
        ILoadingUnitItemManager loadingUnitItemManager,
        IGpoUtility gpoUtility,
        IHubContext<TagHub> tagHub,
        IShipmentPublisher shipmentPublisher)
    {
        this.logger = logger;
        this.readerManager = readerManager;
        this.readerService = readerService;
        this.movementManager = movementManager;
        this.movementItemManager = movementItemManager;
        this.loadingUnitManager = loadingUnitManager;
        this.loadingUnitItemManager = loadingUnitItemManager;
        this.gpoUtility = gpoUtility;
        this.tagHub = tagHub;
        this.shipmentPublisher = shipmentPublisher;
    }

    /// <summary>
    /// Consume SubmitTag da tevent (TagData) consumer
    /// Invia response a tevent con TagInStatus
    /// Aggiorna state machine con ITagEvent
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Consume(ConsumeContext<ShipmentTagData> context)
    {
        if (context.Message.CorrelationId == Guid.Empty)
        {
            logger.LogDebug("ShipmentTagConsumer|{CorrelatioId}|Skipping. Correlation id is empty", context.Message.CorrelationId);
            return;
        }

        logger.LogDebug("ShipmentTagConsumer|{CorrelationId}|ShipmentTagData begin consumer", context.Message.CorrelationId);
        logger.LogDebug("ShipmentTagConsumer|{CorrelationId}|ShipmentTagData working on ReaderId: {ReaderId} for Epc: {Epc}", context.Message.CorrelationId, context.Message.ReaderId, context.Message.TagAction.Tag.Epc);

        if (context.Message.ReaderId == Guid.Empty)
        {
            try
            {
                var reader = readerManager.GetByNameFromCache(context.Message.HostName);
                context.Message.ReaderId = reader.Id; // ?? Guid.Empty;

            }
            catch (WaitingApprovalException wex)
            {
                logger.LogError(wex.Message);
                return;
            }
            catch (InvalidLicenseException ilex)
            {
                logger.LogError(ilex.Message);
                return;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex.Message);
                return;
            }
        }

        try
        {
            logger.LogDebug("ShipmentTagConsumer|{CorrelationId}|Begin Consume for ReaderId {ReaderId} Epc: {Epc}", context.Message.CorrelationId, context.Message.ReaderId, context.Message.TagAction.Tag.Epc);

            var tagRequest = context.Message;
            logger.LogTrace($"ShipmentTagConsumer|{context.Message.CorrelationId}|TagShipmentRequest {@tagRequest}", tagRequest);

            TagActionDto tag = tagRequest.TagAction;
            logger.LogTrace($"ShipmentTagConsumer|{context.Message.CorrelationId}|TagActionDto {@tag}", tag);

            if (tag is null)
            {
                logger.LogDebug("ShipmentTagConsumer|{CorrelationId}|No tag from context {@context}", context.Message.CorrelationId, context);
                return;
            }

            MovementDto movement;
            var movementId = context.Message.MovementId;
            if (context.Message.MovementId == Guid.Empty)
            {
                movement = await movementManager.GetActiveMovement(context.Message.ReaderId, WorkflowType.Shipment);
                if (movement is null)
                {
                    logger.LogWarning("ShipmentTagConsumer|{CorrelationId}|GetActiveMovement on ReaderId: {ReaderId} returned NULL", context.Message.CorrelationId, context.Message.ReaderId);
                    return;
                }

                movementId = movement.Id;
                logger.LogTrace($"ShipmentTagConsumer|{context.Message.CorrelationId}|getting movement info {@movement}", movement);
            }

            logger.LogTrace("ShipmentTagConsumer|{CorrelationId}|AddItemForShipment TagAction: {Action} tag: {@tag}", context.Message.CorrelationId, tag.Action, tag);

            var movementItem = await movementItemManager.AddItemForShipment(movementId, tag, context.Message.ReaderId);
            if (movementItem is null)
            {
                logger.LogWarning("ShipmentTagConsumer|{CorrelationId}|AddItemForShipment result is null", context.Message.CorrelationId);

                //var errResponse = new TagShipmentSMResponse<MovementDto>
                //{
                //    ReaderId = context.Message.ReaderId,
                //    SessionId = context.Message.CorrelationId,
                //    ResponseData = ResponseDataFactory.Exception<MovementDto>("MovementItem is null")
                //};
                //await context.RespondAsync<TagShipmentSMResponse<MovementDto>>(errResponse);
                //logger.LogDebug("ShipmentTagConsumer|{CorrelationId}|Response sent", context.Message.CorrelationId);
                return;
            }

            if (movementItem.Status == ItemStatus.Unexpected && movementItem.IgnoreUntil < DateTime.Now)
            {
                await gpoUtility.SetBuzzerGpo(context.Message.ReaderId, true);
                await gpoUtility.SetLedYellowGpo(context.Message.ReaderId, true);

                await context.Publish<IGpoBuzzerEvent>(new
                {
                    CorrelationId = context.Message.CorrelationId,
                    ReaderId = context.Message.ReaderId,
                    GpoBuzzerId = readerService.GetReaderCache(context.Message.ReaderId).GpioConfiguration.GpOutBuzzer.Id,
                    GpoBuzzerValue = readerService.GetReaderCache(context.Message.ReaderId).GpioConfiguration.GpOutBuzzer.LogicOn,
                    Timestamp = DateTime.Now,
                });

                logger.LogDebug("ShipmentTagConsumer|{CorrelationId}|Unexpected item found for Epc: {Epc}", context.Message.CorrelationId, movementItem.Epc);

                //spostato su publisher e consumer dentro al bridge
                //var loadingUnit = await loadingUnitManager.GetLastActive(WorkflowType.Shipment);
                //if (loadingUnit is null)
                //{
                //    logger.LogWarning("ShipmentConsolidateConsumer|No active loading unit for {ReaderId}", context.Message.ReaderId);
                //    return;
                //}

                //var newLoadinUnitItem = new LoadingUnitItemDto
                //{
                //    Name = movementItem.Name,
                //    Epc = movementItem.Epc,
                //    LoadingUnitId = loadingUnit.Id,
                //    Status = ItemStatus.Unexpected,
                //    Sent = false,

                //};

                //var lui = await loadingUnitItemManager.AddAsync(newLoadinUnitItem);
                //movementItem.LoadingUnitItemId = lui.Id;

                //logger.LogDebug("ShipmentTagConsumer|Unexpected Item Added: {Epc}", newLoadinUnitItem.Epc);

            }

            //aggiorno LUI corrispondente
            if (movementItem.Sent == false && movementItem.LoadingUnitItemId is not null)
            {
                if (movementItem.LoadingUnitItemId != Guid.Empty)
                {
                    try
                    {
                        logger.LogDebug("ShipmentTagConsumer|{CorrelationId}|Publishing BridgeShipmentTagData: {Epc} Status: {Status}", context.Message.CorrelationId, movementItem.Epc, movementItem.Status);

                        var response = await shipmentPublisher.Send(new BridgeShipmentTagRequest
                        {
                            ReaderId = context.Message.ReaderId,
                            LocationReference = readerService.GetReaderCache(context.Message.ReaderId).Reader.LocationReference,
                            MovementId = movementId,
                            TagAction = new TagActionDto
                            {
                                Action = movementItem.ItemStatusEnum.Value,
                                IsTransfered = movementItem.Sent,
                                IsValid = movementItem.IsValid,
                                IgnoreUntil = movementItem.IgnoreUntil,

                                Tag = new TagDto
                                {
                                    ItemStatus = movementItem.Status,
                                    Epc = movementItem.Epc,
                                    //Format = context.Message.Format,
                                    TagSeenCount = movementItem.ReadsCount,

                                    //Phase = context.Message.Phase,
                                    //UserMemory = context.Message.UserMemory,
                                    //Channel = context.Message.Channel,
                                    PC = movementItem.PC,
                                    //Xpc = context.Message.Xpc,
                                    //Crc = context.Message.Crc,
                                    Tid = movementItem.Tid,
                                    Rssi = movementItem.Rssi,
                                    //UserDefined = context.Message.UserDefined,

                                    Timestamp = movementItem.FirstRead
                                }
                            }
                        });

                        if (response.ResponseData.Status == ResponseDataStatus.Ok)
                        {
                            logger.LogDebug("ShipmentTagConsumer|{CorrelationId}|Updating LUI: {LoadingUnitItemId} for {Epc}", context.Message.CorrelationId, movementItem.LoadingUnitItemId, movementItem.Epc);

                            //aggiorno lUI status
                            await loadingUnitItemManager.UpdateStatusAsync(movementItem.LoadingUnitItemId ?? Guid.Empty, movementItem.Status, true);

                            //aggiorno movement item
                            movementItem.Sent = true;
                            await movementItemManager.UpdateAsync(movementItem);
                        }

                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning("ShipmentTagConsumer|{CorrelationId}|Warning update LUI: {LoadingUnitItemId}\n{Message}", context.Message.CorrelationId, movementItem.LoadingUnitItemId, ex.Message);
                        return;
                        //logger.LogTrace("ShipmentTagConsumer|{CorrelationId}|Warning update LUI: {LoadingUnitItemId}\n{Message}", context.Message.CorrelationId, movementItem.LoadingUnitItemId, ex.Message);
                    }
                }
            }
            else if (movementItem.Sent == false && movementItem.Status == ItemStatus.Unexpected)
            {
                //caso unexpected
                try
                {
                    logger.LogDebug("ShipmentTagConsumer|{CorrelationId}|Publishing BridgeShipmentTagData: {Epc} Status: {Status}", context.Message.CorrelationId, movementItem.Epc, movementItem.Status);


                    var response = await shipmentPublisher.Send(new BridgeShipmentTagRequest
                    {
                        ReaderId = context.Message.ReaderId,
                        LocationReference = readerService.GetReaderCache(context.Message.ReaderId).Reader.LocationReference,
                        MovementId = movementId,
                        TagAction = new TagActionDto
                        {
                            Action = movementItem.ItemStatusEnum.Value,
                            IsTransfered = movementItem.Sent,
                            IsValid = movementItem.IsValid,
                            IgnoreUntil = movementItem.IgnoreUntil,

                            Tag = new TagDto
                            {
                                ItemStatus = movementItem.Status,
                                Epc = movementItem.Epc,
                                //Format = context.Message.Format,
                                TagSeenCount = movementItem.ReadsCount,

                                //Phase = context.Message.Phase,
                                //UserMemory = context.Message.UserMemory,
                                //Channel = context.Message.Channel,
                                PC = movementItem.PC,
                                //Xpc = context.Message.Xpc,
                                //Crc = context.Message.Crc,
                                Tid = movementItem.Tid,
                                Rssi = movementItem.Rssi,
                                //UserDefined = context.Message.UserDefined,

                                Timestamp = movementItem.FirstRead
                            }
                        }
                    });


                    if (response.ResponseData.Status == ResponseDataStatus.Ok)
                    {
                        logger.LogDebug("ShipmentTagConsumer|{CorrelationId}|Updating LUI: {LoadingUnitItemId} for {Epc}", context.Message.CorrelationId, movementItem.LoadingUnitItemId, movementItem.Epc);

                        var loadingUnit = await loadingUnitManager.GetLastActive(WorkflowType.Shipment, context.Message.ReaderId);

                        var loadingUnitItem = loadingUnit.LoadingUnitItems.Where(x => x.Epc.ToUpper() == movementItem.Epc.ToUpper()).FirstOrDefault();

                        if (loadingUnitItem is null)
                        {
                            //creo lUI
                            loadingUnitItem = await loadingUnitItemManager.AddAsync(new LoadingUnitItemDto
                            {
                                Name = movementItem.Epc.ToString(),
                                Description = movementItem.Epc.ToString(),
                                Epc = movementItem.Epc.ToString(),
                                Status = movementItem.Status,
                                Sent = true,
                                LoadingUnitId = loadingUnit.Id,
                            });
                        }

                        //aggiorno movement item
                        movementItem.Sent = true;
                        movementItem.LoadingUnitItemId = loadingUnitItem.Id;

                        await movementItemManager.UpdateAsync(movementItem);
                    }



                }
                catch (Exception ex)
                {
                    logger.LogWarning("ShipmentTagConsumer|{CorrelationId}|Warning update LUI: {LoadingUnitItemId}\n{Message}", context.Message.CorrelationId, movementItem.LoadingUnitItemId, ex.Message);
                    return;
                    //logger.LogTrace("ShipmentTagConsumer|{CorrelationId}|Warning update LUI: {LoadingUnitItemId}\n{Message}", context.Message.CorrelationId, movementItem.LoadingUnitItemId, ex.Message);
                }
            }

            logger.LogTrace("ShipmentTagConsumer|{CorrelationId}|Result MovementItem: {@movementItem}", context.Message.CorrelationId, movementItem);

            //dto per scambio dati con la parte frontend
            MovementShipmentDto movementShipmentDto = new MovementShipmentDto()
            {
                ItemDescription = movementItem.Description,
                ItemEpc = movementItem.Epc,
                ItemId = movementItem.Id,
                ItemIsMissingComponent = movementItem.Status == ItemStatus.NotFound,
                ItemIsOverflowComponent = movementItem.Status == ItemStatus.Overflow,
                ItemIsUnexpectedComponent = movementItem.Status == ItemStatus.Unexpected,
                ItemIsValidComponent = movementItem.Status == ItemStatus.Found,
                ItemName = movementItem.Name,
                ItemStatus = movementItem.Status,
                ItemTimestamp = DateTime.Now,
                ItemFirstRead = movementItem.FirstRead,
                ItemLastRead = movementItem.LastRead,
                ItemReadsCount = movementItem.ReadsCount,
                ItemRssi = movementItem.Rssi,
                ItemPC = movementItem.PC,
                MovementId = movementItem.MovementId,
                ReaderId = movementItem.ReaderId,

            };
            logger.LogTrace("ShipmentTagConsumer|{CorrelationId}|MovementShipmentDto {@movementShipmentDto}", context.Message.CorrelationId, movementShipmentDto);

            //invio a singolo gruppo
            await tagHub.Clients.Groups(context.Message.ReaderId.ToString()).SendAsync("TagShipmentRequest", movementShipmentDto);
            ////broadcast a tutti i client
            //await tagHub.Clients.All.SendAsync("GlobalTagComponentRequest", readerTagDto);
            //logger.LogDebug($"GlobalTagComponentRequest");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
        }
        finally
        {
            logger.LogDebug("ShipmentTagConsumer|{CorrelationId}|End Consume for ReaderId {ReaderId} Epc: {Epc}", context.Message.CorrelationId, context.Message.ReaderId, context.Message.TagAction.Tag.Epc);
        }


        logger.LogDebug("ShipmentTagConsumer|{CorrelationId}|ShipmentTagData end consumer", context.Message.CorrelationId);
    }

}
