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
    public async Task<IActionResult> Add(AddFlowerFormDto dto, long cuttingLandId)
    {
        await flowerRepo.AddAsync(dto, cuttingLandId);
        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(DateTime? from, DateTime? to, long? cuttingLandId, string cuttingTitle,
        string cuttingColorCode, string worker, double? Long, int pageSize = 10, int pageNum = 0)
    {
        var Data = await flowerRepo.GetAllAsync(from, to, cuttingLandId, cuttingTitle, cuttingColorCode, worker, Long, pageSize, pageNum);
        var totalCount = Data.Data.Sum(x => x.Count);
        return Ok(new { Data, totalCount });
    }
    [HttpGet]
    public async Task<IActionResult> GetById(long id)
    {
        var x = await flowerRepo.GetByIdAsync(id);
        return Ok(x);
    }
    [HttpGet]
    public async Task<IActionResult> GetFlowerAverageInDonum()
    {
        var avg = await flowerRepo.GetFlowerAverageInDonumAsync();
        return Ok(avg);
    }
    [HttpPost]
    public async Task<IActionResult> Update(long id, string note, string worker, double Long, int count, DateTime date)
    {
        await flowerRepo.UpdateAsync(id, note, worker, Long, count, date);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> Remove(long id)
    {
        await flowerRepo.RemoveAsync(id);
        return Ok();
    }
    #region FlowerStore

    [HttpGet]
    public async Task<IActionResult> GetAllFlowerStore(string code, int pageSize = 10, int pageNum = 0)
    {
        var result = await flowerRepo.GetAllFlowerStoreAsync(code, pageSize, pageNum);
        return Ok(result);
    }
    #endregion
}
