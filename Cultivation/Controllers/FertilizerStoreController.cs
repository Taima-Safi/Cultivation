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
    public async Task<IActionResult> Add(long fertilizerId, double quantity, DateTime date, bool isAdd)
    {
        await fertilizerRepo.AddToStoreAsync(fertilizerId, quantity, date, isAdd);
        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> GetFertilizerTransaction(string fertilizerTitle, DateTime? from, DateTime? to,
        int pageSize = 10, int pageNum = 0)
    {
        var result = await fertilizerRepo.GetAllFertilizerTransactionAsync(fertilizerTitle, from, to, pageSize, pageNum);
        return Ok(result);
    }
}
