namespace Cultivation.Dto.Order;

public class UpdateOrderDto
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public DateTime OrderDate { get; set; }
}
