using Cultivation.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cultivation.Repository.DataBase;

public class DbRepo : IDbRepo
{
    protected DbContext Context { get; }
    protected IDbContextTransaction ContextTransaction { get; set; }

    public DbRepo(CultivationDbContext context)
    {
        Context = context;
    }

    public int SaveChanges()
    {
        return Context.SaveChanges();
    }
    public async Task<int> SaveChangesAsync()
    {
        return await Context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        ContextTransaction = await Context.Database.BeginTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await ContextTransaction.RollbackAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await ContextTransaction.CommitAsync();
    }
    public void CommitTransaction()
    {
        ContextTransaction.Commit();
    }

    public async Task DisposeTransactionAsync()
    {
        await ContextTransaction.DisposeAsync();
    }
}
