using Cultivation.Dto.Order;
using Cultivation.Repository.Order;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderRepo orderRepo;

    public OrderController(IOrderRepo orderRepo)
    {
        this.orderRepo = orderRepo;
    }
    [HttpPost]
    public async Task<IActionResult> Add(OrderFormDto dto)
    {
        await orderRepo.AddAsync(dto);
        return Ok();
    }
}
