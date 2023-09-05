using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data;
using System.Linq.Expressions;
using XRFID.Sample.Webservice.Database;
using XRFID.Sample.Webservice.Entities;

namespace XRFID.Sample.Webservice.Repositories;

public abstract class BaseRepository<T> where T : AuditEntity
{
    private readonly DbSet<T> _table;
    private readonly ILogger _logger;

    protected BaseRepository(XRFIDSampleContext context, ILogger logger)
    {
        _table = context.Set<T>();
        _logger = logger;
    }

    public async Task<List<T>> GetAsync()
    {
        return await _table.ToListAsync();
    }

    public async Task<List<T>> GetAsync(Expression<Func<T, bool>> query)
    {
        return await _table.Where(query).ToListAsync();
    }

    public async Task<T?> GetAsync(Guid id)
    {
        return await _table.FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<T> CreateAsync(T entity)
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

    public async Task<List<T>> CreateAsync(List<T> entities)
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

    public async Task<T> DeleteAsync(T entity)
    {
        if (!await _table.AnyAsync(c => c.Id == entity.Id))
        {
            throw new KeyNotFoundException("Resource not found");
        }

        int affectedRows = await _table.Where(c => c.Id == entity.Id).ExecuteDeleteAsync();

        if (affectedRows == 0)
        {
            throw new Exception("database has failed to delete entity");
        }
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        T? existingEntity = await _table.FirstOrDefaultAsync(c => c.Id == entity.Id);
        if (existingEntity is null)
        {
            throw new KeyNotFoundException("Resource not found");
        }

        //to do use a modified Deepcopy to procedurally update?

        return existingEntity;
    }
}