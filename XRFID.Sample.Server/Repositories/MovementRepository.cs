﻿using Microsoft.EntityFrameworkCore;
using Xerum.XFramework.Common.Enums;
using XRFID.Sample.Server.Database;
using XRFID.Sample.Server.Entities;

namespace XRFID.Sample.Server.Repositories;

public class MovementRepository : BaseRepository<Movement>
{
    public MovementRepository(XRFIDSampleContext context, ILogger<MovementRepository> logger) : base(context, logger)
    {
    }

    public async Task<List<Movement>> DeactivateByReaderId(Guid readerId)
    {
        List<Movement> movments = await GetAsync(g => g.ReaderId == readerId);
        List<Movement> updatedMovments = new List<Movement>();
        foreach (Movement movment in movments)
        {
            movment.IsActive = false;
            updatedMovments.Add(await UpdateAsync(movment));
        }
        return updatedMovments;
    }

    public async Task<Movement> UpdateStatusAsync(Guid movementId)
    {
        Movement movWithItems = await _table.Where(w => w.Id == movementId)
                                            .Include("MovementItems")
                                            .FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Movement with id {movementId} NOT found");

        IEnumerable<MovementItem> rowsWithErrors = movWithItems.MovementItems.Where(q => q.Status == ItemStatus.NotFound || q.Status == ItemStatus.Unexpected || q.Status == ItemStatus.Overflow);

        if (!rowsWithErrors.Any())
        {
            movWithItems.MissingItem = false;
            movWithItems.UnexpectedItem = false;
            movWithItems.OverflowItem = false;
            movWithItems.IsValid = true;
        }
        else
        {
            if (rowsWithErrors.Where(x => x.Status == ItemStatus.NotFound).Count() > 0)
            {
                movWithItems.MissingItem = true;
            }
            else
            {
                movWithItems.MissingItem = false;
            }
            if (rowsWithErrors.Where(x => x.Status == ItemStatus.Unexpected).Count() > 0)
            {
                movWithItems.UnexpectedItem = true;
            }
            else
            {
                movWithItems.UnexpectedItem = false;
            }
            if (rowsWithErrors.Where(x => x.Status == ItemStatus.Overflow).Count() > 0)
            {
                movWithItems.OverflowItem = true;
            }
            else
            {
                movWithItems.OverflowItem = false;
            }
            movWithItems.IsValid = false;
        }

        movWithItems.IsConsolidated = true;

        return await UpdateAsync(movWithItems);
    }
}
