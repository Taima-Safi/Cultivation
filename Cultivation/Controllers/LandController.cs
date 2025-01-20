using Cultivation.Dto.Land;
using Cultivation.Repository.Land;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class LandController : ControllerBase
{
    private readonly ILandRepo landRepo;

    public LandController(ILandRepo landRepo)
    {
        this.landRepo = landRepo;
    }
    [HttpPost]
    public async Task<IActionResult> Add(LandFormDto dto)
        => Ok(await landRepo.AddAsync(dto));

    [HttpGet]
    public async Task<IActionResult> GetAll(string title, bool isActive, double? size, bool justChildren = false)
        => Ok(await landRepo.GetAllAsync(title, null, null, null, null, size, justChildren, isActive, false));
    [HttpGet]
    public async Task<IActionResult> GetById(long id)
        => Ok(await landRepo.GetByIdAsync(id));
    [HttpPost]
    public async Task<IActionResult> Update(long id, LandFormDto dto)
    {
        await landRepo.UpdateAsync(id, dto);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> Remove(long id)
    {
        await landRepo.RemoveAsync(id);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> RemoveAll()
    {
        await landRepo.RemoveAllAsync();
        return Ok();
    }
}
