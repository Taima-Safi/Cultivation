using Cultivation.Dto.CuttingLand;
using Cultivation.Repository.CuttingLand;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class CuttingLandController : ControllerBase
{
    private readonly ICuttingLandRepo cuttingLandRepo;

    public CuttingLandController(ICuttingLandRepo cuttingLandRepo)
    {
        this.cuttingLandRepo = cuttingLandRepo;
    }
    [HttpPost]
    public async Task<IActionResult> Add(CuttingLandFormDto dto)
    {
        var id = await cuttingLandRepo.AddAsync(dto);
        return Ok(id);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(DateTime? date)
    {
        var cuttingLands = await cuttingLandRepo.GetAllAsync(date);
        return Ok(cuttingLands);
    }
    [HttpGet]
    public async Task<IActionResult> GetById(long id)
    {
        var cuttingLand = await cuttingLandRepo.GetByIdAsync(id);
        return Ok(cuttingLand);
    }
    [HttpPost]
    public async Task<IActionResult> Update(long id, DateTime date, long quantity)
    {
        await cuttingLandRepo.UpdateAsync(id, date, quantity);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> Remove(long id)
    {
        await cuttingLandRepo.RemoveAsync(id);
        return Ok();
    }
}
