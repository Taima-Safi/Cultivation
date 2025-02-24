using System.Linq.Expressions;

namespace Cultivation.Repository.Base;

public interface IBaseRepo<T>
{
    Task<bool> CheckIdsAsync(IEnumerable<long> ids);
    Task<bool> CheckIfExistAsync(Expression<Func<T, bool>> expression);
    Task<bool> CheckIfHasNextPageAsync(Expression<Func<T, bool>> expression, int pageSize, int pageNum);
}
