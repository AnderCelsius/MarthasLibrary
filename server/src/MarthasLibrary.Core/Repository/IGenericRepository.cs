namespace MarthasLibrary.Core.Repository;

public interface IGenericRepository<T> where T : class
{
  Task InsertAsync(T entity);
  void Update(T entity);
  void Delete(T entity);
  void InsertRange(List<T> entities);
  void UpdateRange(List<T> entity);
  Task SaveAsync(CancellationToken cancellationToken = default);
  Task SaveAndClearTrackingAsync(CancellationToken cancellationToken = default);


  IQueryable<T> Table { get; }
  IQueryable<T> TableNoTracking { get; }

  // Transaction-related methods
  Task BeginTransactionAsync(CancellationToken cancellationToken);
  Task CommitTransactionAsync(CancellationToken cancellationToken);
  Task RollbackTransactionAsync(CancellationToken cancellationToken);
}