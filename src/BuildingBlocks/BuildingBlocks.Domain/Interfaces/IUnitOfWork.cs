namespace BuildingBlocks.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task<int> SaveChangeAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
