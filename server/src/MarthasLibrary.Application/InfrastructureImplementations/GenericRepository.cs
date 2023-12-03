using MarthasLibrary.Core.Repository;
using MarthasLibrary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.Application.InfrastructureImplementations;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly LibraryDbContext _context;
    private readonly DbSet<T> _dbSet;

    public IQueryable<T> Table => _dbSet;

    public IQueryable<T> TableNoTracking => _dbSet.AsNoTracking();

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        await _context.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        await _context.Database.RollbackTransactionAsync(cancellationToken);
    }

    public GenericRepository(LibraryDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task InsertAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void InsertRange(List<T> entities)
    {
        _dbSet.AddRange(entities);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void UpdateRange(List<T> entity)
    {
        _dbSet.UpdateRange(entity);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAndClearTrackingAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();
    }
}
