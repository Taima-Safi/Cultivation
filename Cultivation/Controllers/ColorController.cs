using Cultivation.Repository.Color;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ColorController : ControllerBase
{
    private readonly IColorRepo colorRepo;

    public ColorController(IColorRepo colorRepo)
    {
        this.colorRepo = colorRepo;
    }
    [HttpPost]
    public async Task<IActionResult> Add(string title, string code)
    {
        var colorId = await colorRepo.AddAsync(title, code);
        return Ok(colorId);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(string title, string code, int pageSize = 10, int pageNum = 0)
    {
        var colors = await colorRepo.GetAllAsync(title, code, pageSize, pageNum);
        return Ok(colors);
    }
    [HttpGet]
    public async Task<IActionResult> GetById(long id)
    {
        var color = await colorRepo.GetByIdAsync(id);
        return Ok(color);
    }
    [HttpPost]
    public async Task<IActionResult> Update(long id, string title, string code)
    {
        await colorRepo.UpdateAsync(id, title, code);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> Remove(long id)
    {
        await colorRepo.RemoveAsync(id);
        return Ok();
    }
}
