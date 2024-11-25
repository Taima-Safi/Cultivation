﻿using Cultivation.Dto.Order;
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
    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(long orderId, DateTime boughtDate)
    {
        await orderRepo.UpdateOrderStatusAsync(orderId, boughtDate);
        return Ok();
    }
    [HttpPost]
    public async Task<IActionResult> Remove(long id)
    {
        await orderRepo.RemoveAsync(id);
        return Ok();
    }
}
