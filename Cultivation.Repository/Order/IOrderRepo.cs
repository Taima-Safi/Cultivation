using Cultivation.Dto.Order;
using FourthPro.Dto.Common;

namespace Cultivation.Repository.Order;

public interface IOrderRepo
{
    Task AddAsync(OrderFormDto dto);
    Task<bool> CheckIfExistAsync(long id);
    Task<CommonResponseDto<List<OrderDto>>> GetAllAsync(bool isBought, DateTime? from, DateTime? to, int pageSize, int pageNum);
    Task RemoveAsync(long id);
    Task UpdateOrderStatusAsync(long orderId, DateTime boughtDate);
}
