using Cultivation.Dto.Order;

namespace Cultivation.Repository.Order;

public interface IOrderRepo
{
    Task AddAsync(OrderFormDto dto);
}
