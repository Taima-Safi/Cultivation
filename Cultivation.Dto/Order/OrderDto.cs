using Cultivation.Dto.Client;

namespace Cultivation.Dto.Order;

public class OrderDto
{
    public long Id { get; set; }
    public string Number { get; set; }
    public bool IsBought { get; set; }

    public ClientDto Client { get; set; }
    //public List<FlowerOrderModel> FlowerOrders { get; set; }
}
