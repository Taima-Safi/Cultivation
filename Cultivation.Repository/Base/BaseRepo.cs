using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cultivation.Repository.Base;

public class BaseRepo<T> : IBaseRepo<T> where T : BaseModel
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
    public async Task<bool> CheckIfExistAsync(Expression<Func<T, bool>> expression)
    => await Entity.Where(expression).AnyAsync();

    public async Task<bool> CheckIdsAsync(IEnumerable<long> ids)
    {
        var entityIdsModel = await Entity.Select(s => s.Id).ToListAsync();
        var check = ids.All(entityIdsModel.Contains);
        return check;
    }
}
