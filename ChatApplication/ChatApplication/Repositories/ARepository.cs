using Database.Context;
using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.Repositories;

public abstract class ARepository<TEntity> where TEntity: class
{
    protected ApplicationContext _context;
    protected DbSet<TEntity> _dbSet;

    public ARepository(ApplicationContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<List<TEntity>> AddRangeAsync(List<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
        return entities;
    }

    public async Task UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<TEntity> entities)
    {
        _dbSet.UpdateRange();
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}