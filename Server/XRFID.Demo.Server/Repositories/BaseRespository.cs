using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;

namespace XRFID.Demo.Server.Repositories;

public class BaseRepository<T> where T : AuditEntity
{
    protected readonly DbSet<T> _table;
    protected readonly ILogger _logger;

    public BaseRepository(XRFIDSampleContext context, ILogger logger)
    {
        _table = context.Set<T>();
        _logger = logger;
    }

    #region Get
    public virtual async Task<List<T>> GetAsync(string? include = null)
    {
        if (include is not null)
        {
            return await _table.Include(include).ToListAsync();
        }

        return await _table.ToListAsync();
    }

    public virtual async Task<List<T>> GetAsync(Expression<Func<T, bool>> query, string? include = null)
    {
        if (include is not null)
        {
            return await _table.Where(query).Include(include).ToListAsync();
        }

        return await _table.Where(query).ToListAsync();
    }

    public virtual async Task<T?> GetAsync(Guid id, string? include = null)
    {
        if (include is not null)
        {
            return await _table.Include(include).FirstOrDefaultAsync(f => f.Id == id);
        }
        return await _table.FirstOrDefaultAsync(f => f.Id == id);
    }
    #endregion

    #region Create
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

        return result.Entity;
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
    #endregion

    #region Delete
    [Obsolete("Use delete by Id as it is more reliable")]
    public virtual async Task<T> DeleteAsync(T entity)
    {
        if (!(await _table.AsNoTracking().AnyAsync(c => c.Id == entity.Id)))
        {
            throw new KeyNotFoundException("Resource not found");
        }

        return _table.Remove(entity).Entity;
    }

    public virtual async Task<T> DeleteAsync(Guid id)
    {
        T entity = await _table.FirstOrDefaultAsync(f => f.Id == id) ?? throw new KeyNotFoundException("Resource not found");
        return _table.Remove(entity).Entity;
    }
    #endregion

    #region Update
    public virtual async Task<T> UpdateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        T existingEntity = await _table.FirstOrDefaultAsync(c => c.Id == entity.Id) ?? throw new KeyNotFoundException("Resource not found");

        //uses reflection to swap out everything inside the Object, without changing the reference to the object itself
        PropertyInfo[] properties = entity.GetType().GetProperties();

        foreach (PropertyInfo property in properties)
        {
            property.SetValue(existingEntity, property.GetValue(entity));
        }
        var result = _table.Update(existingEntity).Entity;
        return result;
    }
    #endregion

    public virtual void Detach(T entity)
    {
        _table.Entry(entity).State = EntityState.Detached;
    }
}