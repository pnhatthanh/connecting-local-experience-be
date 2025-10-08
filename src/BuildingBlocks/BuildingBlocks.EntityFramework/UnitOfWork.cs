using BuildingBlocks.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuildingBlocks.EntityFramework
{
    public class UnitOfWork<TContext> : IUnitOfWork, IDisposable
        where TContext : BaseDbContext
    {
        private readonly TContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(TContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }

        public async Task<int> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}
