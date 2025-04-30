using Cultivation.Repository.Insecticide;
using Cultivation.Shared.Enum;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class InsecticideStoreController : ControllerBase
{
    private readonly IInsecticideRepo insecticideRepo;

    public InsecticideStoreController(IInsecticideRepo insecticideRepo)
    {
        this.insecticideRepo = insecticideRepo;
    }
    //[HttpPost]
    //public async Task<IActionResult> UpdateStore(long insecticideId, double quantity, DateTime date, bool isAdd)
    //{
    //    await insecticideRepo.UpdateStoreAsync(insecticideId, quantity, date, isAdd);
    //    return Ok();
    //}

    [HttpPost]
    public async Task<IActionResult> UpdateStoreForMix(long mixId, double donumNum, DateTime date)
    {
        await insecticideRepo.UpdateStoreForMixAsync(mixId, donumNum, date);
        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> GetAllInsecticideStore(string insecticideTitle, InsecticideType type, DateTime? date,
    int pageSize = 10, int pageNum = 0)
    {
        var result = await insecticideRepo.GetAllInsecticideStoreAsync(insecticideTitle, type, date, pageSize, pageNum);
        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetInsecticideTransaction(string insecticideTitle, DateTime? from, DateTime? to,
        int pageSize = 10, int pageNum = 0)
    {
        var result = await insecticideRepo.GetAllInsecticideTransactionAsync(insecticideTitle, from, to, pageSize, pageNum);
        return Ok(result);
    }
}
