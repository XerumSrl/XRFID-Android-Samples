using MassTransit;
using System.Text.Json;
using Xerum.XFramework.Common.Enums;
using XRFID.Demo.Common.Dto;
using XRFID.Demo.Server.Contracts;
using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;
using XRFID.Demo.Server.Repositories;
using XRFID.Demo.Server.StateMachines.Shipment.Contracts;
using XRFID.Demo.Server.StateMachines.Shipment.Interfaces;
using XRFID.Demo.Server.Utilities;

namespace XRFID.Demo.Server.StateMachines.Shipment.Consumers;

public class ShipmentTagConsumer :
    IConsumer<ShipmentTagData>
{
    private readonly ReaderRepository readerRepository;
    private readonly MovementRepository movementRepository;
    private readonly MovementItemRepository movementItemRepository;
    private readonly UnitOfWork uowk;
    private readonly GpoUtility gpoUtility;
    private readonly ILogger<ShipmentTagConsumer> logger;

    public ShipmentTagConsumer(ReaderRepository readerRepository,
                               MovementRepository movementRepository,
                               MovementItemRepository movementItemRepository,
                               UnitOfWork uowk,
                               GpoUtility gpoUtility,
                               ILogger<ShipmentTagConsumer> logger)
    {
        this.readerRepository = readerRepository;
        this.movementRepository = movementRepository;
        this.movementItemRepository = movementItemRepository;
        this.uowk = uowk;
        this.gpoUtility = gpoUtility;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<ShipmentTagData> context)
    {
        if (context.Message.CorrelationId == Guid.Empty)
        {
            logger.LogDebug("[Consume<ShipmentTagData>] {CorrelatioId}|Skipping. Correlation id is empty", context.Message.CorrelationId);
            return;
        }

        logger.LogDebug("[Consume<ShipmentTagData>] {CorrelationId}|ShipmentTagData begin consumer", context.Message.CorrelationId);
        logger.LogDebug("[Consume<ShipmentTagData>] {CorrelationId}|ShipmentTagData working on ReaderId: {ReaderId} for Epc: {Epc}", context.Message.CorrelationId, context.Message.ReaderId, context.Message.TagAction.Tag.Epc);

        if (context.Message.ReaderId == Guid.Empty)
        {
            try
            {
                Reader? reader = (await readerRepository.GetAsync(q => q.Name == context.Message.HostName)).FirstOrDefault();
                if (reader is null)
                {
                    throw new Exception($"Reader with name {context.Message.HostName} does not exist");
                }
                context.Message.ReaderId = reader.Id;

            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "[Consume<ShipmentTagData>] ");
                return;
            }
        }

        try
        {
            logger.LogDebug("[Consume<ShipmentTagData>] {CorrelationId}|Begin Consume for ReaderId {ReaderId} Epc: {Epc}", context.Message.CorrelationId, context.Message.ReaderId, context.Message.TagAction.Tag.Epc);

            var tagRequest = context.Message;
            logger.LogTrace($"[Consume<ShipmentTagData>] {context.Message.CorrelationId}|TagShipmentRequest {@tagRequest}", tagRequest);

            TagActionDto tag = tagRequest.TagAction;
            logger.LogTrace($"[Consume<ShipmentTagData>] {context.Message.CorrelationId}|TagActionDto {@tag}", tag);

            if (tag is null)
            {
                logger.LogDebug("[Consume<ShipmentTagData>] {CorrelationId}|No tag from context {@context}", context.Message.CorrelationId, context);
                return;
            }

            Movement? movement;
            var movementId = context.Message.MovementId;
            if (context.Message.MovementId == Guid.Empty)
            {
                movement = (await movementRepository.GetAsync(q => q.ReaderId == context.Message.ReaderId && q.IsActive)).FirstOrDefault();
                if (movement is null)
                {
                    logger.LogWarning("[Consume<ShipmentTagData>] {CorrelationId}|GetActiveMovement on ReaderId: {ReaderId} returned NULL", context.Message.CorrelationId, context.Message.ReaderId);
                    return;
                }
                movementId = movement.Id;
                logger.LogTrace($"[Consume<ShipmentTagData>] {context.Message.CorrelationId}|getting movement info {@movement}", movement);
            }

            logger.LogTrace("[Consume<ShipmentTagData>] {CorrelationId}|AddItemForShipment TagAction: {Action} tag: {@tag}", context.Message.CorrelationId, tag.Action, tag);

            #region Save MovementItem

            if (string.IsNullOrEmpty(tag.Tag.Epc))
            {
                throw new Exception("[Consume<ShipmentTagData>] Epc string is empty");
            }
            MovementItem? movementItem = (await movementItemRepository.GetAsync(q => q.MovementId == movementId && q.Epc.ToUpper() == tag.Tag.Epc.ToUpper())).FirstOrDefault();

            if (movementItem is not null && movementItem.Status != ItemStatus.NotFound)
            {
                logger.LogTrace("[Consume<ShipmentTagData>] {Epc}|Begin updating existing item: {@item}", tag.Tag.Epc, movementItem);

                movementItem.LastRead = DateTime.Now;
                movementItem.ReadsCount += tag.Tag.TagSeenCount;
                movementItem.Rssi = tag.Tag.Rssi;
                movementItem.Tid = tag.Tag.Tid;
                movementItem.PC = tag.Tag.PC;
                movementItem.Checked = tag.IsValid;
                movementItem.IgnoreUntil = tag.IgnoreUntil;

                movementItem = await movementItemRepository.UpdateAsync(movementItem);
                await uowk.SaveAsync();
            }
            else if (movementItem is not null && movementItem.Status == ItemStatus.NotFound)
            {
                logger.LogTrace("[Consume<ShipmentTagData>] {Epc}|Begin updating not found item: {@item} to ItemStatus.Found", tag.Tag.Epc, movementItem);

                movementItem.FirstRead = DateTime.Now;
                movementItem.LastRead = DateTime.Now;
                movementItem.ReadsCount += tag.Tag.TagSeenCount;
                movementItem.Status = ItemStatus.Found;

                movementItem.Rssi = tag.Tag.Rssi;
                movementItem.Tid = tag.Tag.Tid;
                movementItem.PC = tag.Tag.PC;
                movementItem.Checked = tag.IsValid;
                movementItem.IgnoreUntil = tag.IgnoreUntil;

                movementItem = await movementItemRepository.UpdateAsync(movementItem);
                await uowk.SaveAsync();

                logger.LogDebug("[Consume<ShipmentTagData>] {Epc}|End updating not found item: {@item} to ItemStatus.Found", tag.Tag.Epc, movementItem);
            }
            else if (movementItem is null)
            {
                logger.LogTrace("[Consume<ShipmentTagData>] {Epc}|Begin adding Unexpected item", tag.Tag.Epc);

                var newItem = new MovementItem();
                newItem.Name = tag.Tag.Epc;
                newItem.Description = ItemStatus.Unexpected.ToString();
                newItem.Epc = tag.Tag.Epc.ToUpper();
                newItem.MovementId = movementId;
                newItem.Status = ItemStatus.Unexpected;
                newItem.ReadsCount = tag.Tag.TagSeenCount;
                newItem.FirstRead = DateTime.Now;
                newItem.LastRead = DateTime.Now;

                newItem.Rssi = tag.Tag.Rssi;
                newItem.Tid = tag.Tag?.Tid;
                newItem.PC = tag.Tag?.PC;
                newItem.Checked = tag.IsValid;
                newItem.IgnoreUntil = DateTime.Now;

                movementItem = await movementItemRepository.CreateAsync(newItem);
                await uowk.SaveAsync();

                logger.LogDebug("[Consume<ShipmentTagData>] {Epc}|End adding Unexpected item: {@item}", tag.Tag.Epc, movementItem);

            }

            #endregion

            if (movementItem is null)
            {
                logger.LogWarning("[Consume<ShipmentTagData>] {CorrelationId}|AddItemForShipment result is null", context.Message.CorrelationId);
                return;
            }

            if (movementItem.Status == ItemStatus.Unexpected && movementItem.IgnoreUntil < DateTime.Now)
            {
                Reader? reader = (await readerRepository.GetAsync(q => q.Id == context.Message.ReaderId)).FirstOrDefault();
                if (reader is not null && reader.Name is not null && reader.GpoConfiguration is not null)
                {
                    GpoConfiguration? gpo = new();
                    gpo = JsonSerializer.Deserialize<GpoConfiguration>(reader.GpoConfiguration);
                    if (gpo is not null)
                    {
                        await gpoUtility.SetGpo(reader.Id, reader.Name, gpo.YellowLed, true); //Yellow on

                        await gpoUtility.SetGpo(reader.Id, reader.Name, gpo.Buzzer, true); //Buzzer on

                        await context.Publish<IGpoBuzzerEvent>(new
                        {
                            CorrelationId = context.Message.CorrelationId,
                            ReaderId = context.Message.ReaderId,
                            GpoBuzzerId = gpo.Buzzer,
                            GpoBuzzerValue = true,
                            Timestamp = DateTime.Now,
                        });
                    }
                }


                logger.LogDebug("[Consume<ShipmentTagData>] {CorrelationId}|Unexpected item found for Epc: {Epc}", context.Message.CorrelationId, movementItem.Epc);
            }

            //Send message to update UI
            await context.Publish(new StateMachineUiTagPublish
            {
                ReaderId = context.Message.ReaderId,
                ActivMoveId = movementId,
                Tag = context.Message.TagAction,
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
        }
        finally
        {
            logger.LogDebug("[Consume<ShipmentTagData>] {CorrelationId}|End Consume for ReaderId {ReaderId} Epc: {Epc}", context.Message.CorrelationId, context.Message.ReaderId, context.Message.TagAction.Tag.Epc);
        }


        logger.LogDebug("[Consume<ShipmentTagData>] {CorrelationId}|ShipmentTagData end consumer", context.Message.CorrelationId);
    }

}
