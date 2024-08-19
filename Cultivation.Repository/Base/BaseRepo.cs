using Cultivation.Database.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cultivation.Repository.Base;

public class BaseRepo<T> : IBaseRepo<T> where T : class
{
    protected readonly CultivationDbContext context;
    protected readonly DbSet<T> Entity;
    public BaseRepo(CultivationDbContext context)
    {
        this.context = context;
        Entity = context.Set<T>();
    }
    public async Task<bool> CheckIfHasNextPageAsync(Expression<Func<T, bool>> expression, int pageSize, int pageNum)
    {
        var totalRecords = await Entity.Where(expression).CountAsync();
        ++pageNum;
        return totalRecords > (pageSize * pageNum);
    }
}
