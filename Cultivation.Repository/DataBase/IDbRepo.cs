
namespace Cultivation.Repository.DataBase;

public interface IDbRepo
{
    Task BeginTransactionAsync();
    void CommitTransaction();
    Task CommitTransactionAsync();
    Task DisposeTransactionAsync();
    Task RollbackTransactionAsync();
    int SaveChanges();
    Task<int> SaveChangesAsync();
}
