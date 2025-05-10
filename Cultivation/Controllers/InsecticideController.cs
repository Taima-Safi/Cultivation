using Cultivation.Dto.Insecticide;
using Cultivation.Repository.Insecticide;
using Cultivation.Repository.InsecticideMix;
using Cultivation.Shared.Enum;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class InsecticideController : ControllerBase
{
    private readonly IInsecticideRepo insecticideRepo;
    private readonly IInsecticideMixRepo mixRepo;

    public InsecticideController(IInsecticideRepo insecticideRepo, IInsecticideMixRepo mixRepo)
    {
        this.insecticideRepo = insecticideRepo;
        this.mixRepo = mixRepo;
    }
    [HttpPost]
    public async Task<IActionResult> Add(InsecticideFormDto dto)
    {
        var id = await insecticideRepo.AddAsync(dto);
        return Ok(id);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(string title, string publicTitle, string note,
        InsecticideType? type, int pageSize = 10, int pageNum = 0)
    {
        var insecticides = await insecticideRepo.GetAllAsync(title, publicTitle, note, type, pageSize, pageNum);
        return Ok(insecticides);
    }
    [HttpGet]
    public async Task<IActionResult> GetById(long id)
    {
        var insecticide = await insecticideRepo.GetByIdAsync(id);
        return Ok(insecticide);
    }
    [HttpPost]
    public async Task<IActionResult> Update(long id, InsecticideFormDto dto)
    {
        await insecticideRepo.UpdateAsync(id, dto);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> Remove(long id)
    {
        await insecticideRepo.RemoveAsync(id);
        return Ok();
    }
    #region Mix
    [HttpPost]
    public async Task<IActionResult> AddMix(InsecticideMixFormDto dto)
    {
        await mixRepo.AddAsync(dto);
        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> GetAllMixes(string title, string note, int pageSize = 10, int pageNum = 0)
    {
        var mixes = await mixRepo.GetAllAsync(title, note, pageSize, pageNum);
        return Ok(mixes);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllInsecticideApplicableMix()
    {
        var result = await mixRepo.GetAllInsecticideApplicableMixAsync();
        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetMixById(long id)
    {
        var mix = await mixRepo.GetByIdAsync(id);
        return Ok(mix);
    }
    [HttpPost]
    public async Task<IActionResult> UpdateMix(long id, string title, string note, ColorType color)
    {
        await mixRepo.UpdateAsync(id, title, note, color);
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
