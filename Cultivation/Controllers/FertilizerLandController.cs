using Cultivation.Dto.FertilizerLand;
using Cultivation.Repository.FertilizerLand;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class FertilizerLandController : ControllerBase
{
    private readonly IFertilizerLandRepo fertilizerLandRepo;

    public FertilizerLandController(IFertilizerLandRepo fertilizerLandRepo)
    {
        this.fertilizerLandRepo = fertilizerLandRepo;
    }
    [HttpPost]
    public async Task<IActionResult> Add(FertilizerLandFormDto dto)
    {
        await fertilizerLandRepo.AddAsync(dto);
        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(int pageSize = 10, int pageNum = 0)
    {
        var result = await fertilizerLandRepo.GetAllAsync(pageSize, pageNum);
        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetFertilizersLand(long landId, DateTime? from, DateTime? to, int pageSize = 10, int pageNum = 0)
    {
        var result = await fertilizerLandRepo.GetFertilizersLandAsync(landId, from, to, pageSize, pageNum);
        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await fertilizerLandRepo.GetByIdAsync(id);
        return Ok(result);
    }
    [HttpPost]
    public async Task<IActionResult> Update(long id, UpdateFertilizerLandDto dto)
    {
        await fertilizerLandRepo.UpdateAsync(id, dto);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> Remove(long id)
    {
        await fertilizerLandRepo.RemoveAsync(id);
        return Ok();
    }
}
