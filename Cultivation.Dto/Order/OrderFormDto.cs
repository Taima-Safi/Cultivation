namespace Cultivation.Dto.Order;

public class OrderFormDto
{
    public string Number { get; set; }
    public bool IsBought { get; set; }
    public long ClientId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? BoughtDate { get; set; }

    public List<FlowerOrderDetailDto> FlowerOrderDetails { get; set; } = [];
}
