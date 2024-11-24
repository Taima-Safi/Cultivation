using Cultivation.Dto.Order;

namespace Cultivation.Dto.Client;

public class ClientDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool IsLocal { get; set; }
    public string PhoneNumber { get; set; }
    public string CodePhoneNumber { get; set; }
    public List<OrderDto> Orders { get; set; }
}
