using Cultivation.Dto.Common;
using Cultivation.Dto.Order;

namespace Cultivation.Repository.Order;

public interface IOrderRepo
{
    Task AddAsync(OrderFormDto dto);
    Task<bool> CheckIfExistAsync(long id);
    Task<CommonResponseDto<List<OrderDto>>> GetAllAsync(bool isBought, DateTime? from, DateTime? to, int pageSize, int pageNum);
    Task<int> GetOrderCountAsync();
    Task RemoveAsync(long id);
    Task UpdateAsync(UpdateOrderDto dto);
    Task UpdateOrderStatusAsync(long orderId, DateTime boughtDate);
}
