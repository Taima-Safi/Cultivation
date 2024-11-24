namespace Cultivation.Dto.Order;

public class OrderFormDto
{
    public string Number { get; set; }
    public bool IsBought { get; set; }
    public long ClientId { get; set; }

    public List<FlowerOrderDto> FlowerOrders { get; set; } = [];
}
