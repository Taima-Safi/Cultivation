using Cultivation.Dto.Order;

namespace Cultivation.Repository.Order;

public interface IOrderRepo
{
    Task AddAsync(OrderFormDto dto);
    Task RemoveAsync(long id);
    Task UpdateOrderStatusAsync(long orderId, DateTime boughtDate);
}
