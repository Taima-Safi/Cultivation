using Cultivation.Dto.Fertilizer;
using Cultivation.Repository.Fertilizer;
using Cultivation.Repository.FertilizerMix;
using Cultivation.Shared.Enum;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class FertilizerController : ControllerBase
{
    private readonly IFertilizerRepo fertilizerRepo;
    private readonly IFertilizerMixRepo mixRepo;

    public FertilizerController(IFertilizerRepo fertilizerRepo, IFertilizerMixRepo mixRepo)
    {
        this.fertilizerRepo = fertilizerRepo;
        this.mixRepo = mixRepo;
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
    #region Mix
    [HttpPost]
    public async Task<IActionResult> AddMix(FertilizerMixFormDto dto)
    {
        await mixRepo.AddAsync(dto);
        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> GetAllMixes(string title, int pageSize = 10, int pageNum = 0)
    {
        var mixes = await mixRepo.GetAllAsync(title, pageSize, pageNum);
        return Ok(mixes);
    }
    [HttpGet]
    public async Task<IActionResult> GetMixById(long id)
    {
        var mix = await mixRepo.GetByIdAsync(id);
        return Ok(mix);
    }
    [HttpPost]
    public async Task<IActionResult> UpdateMix(long id, string title, FertilizerType type, ColorType color)
    {
        await mixRepo.UpdateAsync(id, title, type, color);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> RemoveMix(long id)
    {
        await mixRepo.RemoveAsync(id);
        return Ok();
    }
    #endregion
}
