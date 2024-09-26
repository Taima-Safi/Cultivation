using Cultivation.Repository.Flower;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class FlowerController : ControllerBase
{
    private readonly IFlowerRepo flowerRepo;

    public FlowerController(IFlowerRepo flowerRepo)
    {
        this.flowerRepo = flowerRepo;
    }
    [HttpPost]
    public async Task<IActionResult> Add(int count, string note, DateTime date, long cuttingLandId)
    {
        var id = await flowerRepo.AddAsync(count, note, date, cuttingLandId);
        return Ok(id);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(DateTime? date, long? cuttingLandId)
    {
        var x = await flowerRepo.GetAllAsync(date, cuttingLandId);
        return Ok(x);
    }
    [HttpGet]
    public async Task<IActionResult> GetById(long id)
    {
        var x = await flowerRepo.GetByIdAsync(id);
        return Ok(x);
    }
    [HttpPost]
    public async Task<IActionResult> Update(long id, string note, int count, DateTime date)
    {
        await flowerRepo.UpdateAsync(id, note, count, date);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> Remove(long id)
    {
        await flowerRepo.RemoveAsync(id);
        return Ok();
    }
}
