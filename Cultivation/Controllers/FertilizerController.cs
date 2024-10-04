using Cultivation.Dto.Fertilizer;
using Cultivation.Repository.Fertilizer;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class FertilizerController : ControllerBase
{
    private readonly IFertilizerRepo fertilizerRepo;

    public FertilizerController(IFertilizerRepo fertilizerRepo)
    {
        this.fertilizerRepo = fertilizerRepo;
    }

    [HttpPost]
    public async Task<IActionResult> Add(FertilizerFormDto dto)
    {
        var id = await fertilizerRepo.AddAsync(dto);
        return Ok(id);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(string npk, string title, string publicTitle, string description, int pageSize = 10, int pageNum = 0)
    {
        var fertilizers = await fertilizerRepo.GetAllAsync(npk, title, publicTitle, description, pageSize, pageNum);
        return Ok(fertilizers);
    }
    [HttpGet]
    public async Task<IActionResult> GetById(long id)
    {
        var fertilizer = await fertilizerRepo.GetByIdAsync(id);
        return Ok(fertilizer);
    }
    [HttpPost]
    public async Task<IActionResult> Update(long id, FertilizerFormDto dto)
    {
        await fertilizerRepo.UpdateAsync(id, dto);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> Remove(long id)
    {
        await fertilizerRepo.RemoveAsync(id);
        return Ok();
    }
}
