using Cultivation.Dto.Insecticide;
using Cultivation.Repository.Insecticide;
using Cultivation.Shared.Enum;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class InsecticideController : ControllerBase
{
    private readonly IInsecticideRepo insecticideRepo;

    public InsecticideController(IInsecticideRepo insecticideRepo)
    {
        this.insecticideRepo = insecticideRepo;
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
}
