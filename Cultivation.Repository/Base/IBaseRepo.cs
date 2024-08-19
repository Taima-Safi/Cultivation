using System.Linq.Expressions;

namespace Cultivation.Repository.Base;

public interface IBaseRepo<T>
{
    Task<bool> CheckIfHasNextPageAsync(Expression<Func<T, bool>> expression, int pageSize, int pageNum);
}
