using Cultivation.Repository.Fertilizer;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class FertilizerStoreController : ControllerBase
{
    private readonly IFertilizerRepo fertilizerRepo;

    public FertilizerStoreController(IFertilizerRepo fertilizerRepo)
    {
        this.fertilizerRepo = fertilizerRepo;
    }
    [HttpPost]
    public async Task<IActionResult> UpdateStore(long fertilizerId, double quantity, DateTime date, bool isAdd)
    {
        await fertilizerRepo.UpdateStoreAsync(fertilizerId, quantity, date, isAdd);
        return Ok();
    }
    //[HttpPost]
    //public async Task<IActionResult> UpdateStoreForMix(long mixId, double donumNum, DateTime date)
    //{
    //    await fertilizerRepo.UpdateStoreForMixAsync(mixId, donumNum, date);
    //    return Ok();
    //}
    [HttpGet]
    public async Task<IActionResult> GetAllFertilizerStore(string fertilizerTitle, string npk, DateTime? date,
        int pageSize = 10, int pageNum = 0)
    {
        var result = await fertilizerRepo.GetAllFertilizerStoreAsync(fertilizerTitle, npk, date, pageSize, pageNum);
        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetFertilizerTransaction(string fertilizerTitle, DateTime? from, DateTime? to,
        int pageSize = 10, int pageNum = 0)
    {
        var result = await fertilizerRepo.GetAllFertilizerTransactionAsync(fertilizerTitle, from, to, pageSize, pageNum);
        return Ok(result);
    }
}
