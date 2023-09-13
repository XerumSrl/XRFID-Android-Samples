using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using XRFID.Sample.Common.Utils;
using XRFID.Sample.Server.Database;
using XRFID.Sample.Server.Entities;

namespace XRFID.Sample.Server.Repositories;

public class BaseRepository<T> where T : AuditEntity
{
    private readonly DbSet<T> _table;
    private readonly ILogger _logger;

    public BaseRepository(XRFIDSampleContext context, ILogger logger)
    {
        _table = context.Set<T>();
        _logger = logger;
    }

    public virtual async Task<List<T>> GetAsync()
    {
        return await _table.ToListAsync();
    }

    public virtual async Task<List<T>> GetAsync(Expression<Func<T, bool>> query)
    {
        return await _table.Where(query).ToListAsync();
    }

    public virtual async Task<T?> GetAsync(Guid id)
    {
        return await _table.FirstOrDefaultAsync(f => f.Id == id);
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (await _table.AnyAsync(c => c.Id == entity.Id))
        {
            throw new DuplicateNameException();
        }

        EntityEntry<T> result = await _table.AddAsync(entity);

        return ObjectUtils.DeepCopy(result.Entity);
    }

    public virtual async Task<List<T>> CreateAsync(List<T> entities)
    {
        foreach (T entity in entities)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (await _table.AnyAsync(c => c.Id == entity.Id))
            {
                throw new DuplicateNameException();
            }
        }
        List<T> result = new List<T>();
        foreach (T entity in entities)
        {
            result.Add((await _table.AddAsync(entity)).Entity);
        }
        return result;
    }

    public virtual async Task<T> DeleteAsync(T entity)
    {
        if (!(await _table.AsNoTracking().AnyAsync(c => c.Id == entity.Id)))
        {
            throw new KeyNotFoundException("Resource not found");
        }

        _table.Remove(entity);

        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        T? existingEntity = await _table.FirstOrDefaultAsync(c => c.Id == entity.Id);
        if (existingEntity is null)
        {
            throw new KeyNotFoundException("Resource not found");
        }

        #region UpdateCode
        PropertyInfo[] properties = entity.GetType().GetProperties();

        foreach (PropertyInfo property in properties)
        {
            property.SetValue(existingEntity, property.GetValue(entity));
        }
        #endregion
        var result = _table.Update(existingEntity).Entity;
        return result;
    }
}