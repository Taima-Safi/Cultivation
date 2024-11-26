using Cultivation.Dto.Client;

namespace Cultivation.Dto.Order;

public class OrderDto
{
    public long Id { get; set; }
    public string Number { get; set; }
    public bool IsBought { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? BoughtDate { get; set; }

    public ClientDto Client { get; set; }
    public List<OrderDetailDto> OrderDetails { get; set; } = [];
}
