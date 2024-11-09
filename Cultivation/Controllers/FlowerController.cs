using Cultivation.Dto.Flower;
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
    public async Task<IActionResult> Add(List<FlowerFormDto> dtos, DateTime date, long cuttingLandId)
    {
        await flowerRepo.AddAsync(dtos, date, cuttingLandId);
        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(DateTime? from, DateTime? to, long? cuttingLandId, string cuttingTitle,
        string cuttingColorCode, string colorTitle, double? Long, int pageSize = 10, int pageNum = 0)
    {
        var Data = await flowerRepo.GetAllAsync(from, to, cuttingLandId, cuttingTitle, cuttingColorCode, colorTitle, Long, pageSize, pageNum);
        var totalCount = Data.Data.Sum(x => x.Count);
        return Ok(new { Data, totalCount });
    }
    [HttpGet]
    public async Task<IActionResult> GetById(long id)
    {
        var x = await flowerRepo.GetByIdAsync(id);
        return Ok(x);
    }
    [HttpPost]
    public async Task<IActionResult> Update(long id, string note, double Long, int count, DateTime date)
    {
        await flowerRepo.UpdateAsync(id, note, Long, count, date);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> Remove(long id)
    {
        await flowerRepo.RemoveAsync(id);
        return Ok();
    }
}
