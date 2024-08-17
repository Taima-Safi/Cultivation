using Cultivation.Dto.Cutting;
using Cultivation.Repository.Cutting;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class CuttingController : ControllerBase
{
    private readonly ICuttingRepo cuttingRepo;

    public CuttingController(ICuttingRepo cuttingRepo)
    {
        this.cuttingRepo = cuttingRepo;
    }
    [HttpPost]
    public async Task<IActionResult> Add(string title, string type, int age)
    {
        var cuttingId = await cuttingRepo.AddAsync(title, type, age);
        return Ok(cuttingId);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(string title, string type, int? age)
    {
        var cuttings = await cuttingRepo.GetAllAsync(title, type, age);
        return Ok(cuttings);
    }
    [HttpGet]
    public async Task<IActionResult> GetById(long id)
    {
        var cutting = await cuttingRepo.GetByIdAsync(id);
        return Ok(cutting);
    }
    [HttpPost]
    public async Task<IActionResult> Update(long id, string title, string type, int age)
    {
        await cuttingRepo.UpdateAsync(id, title, type, age);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> Remove(long id)
    {
        await cuttingRepo.RemoveAsync(id);
        return Ok();
    }

    #region CuttingColor
    [HttpPost]
    public async Task<IActionResult> AddCuttingColor(CuttingColorFormDto dto)
    {
        var cuttingColorId = await cuttingRepo.AddCuttingColorAsync(dto);
        return Ok(cuttingColorId);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllCuttingColor(string code)
    {
        var cuttings = await cuttingRepo.GetAllCuttingColorAsync(code);
        return Ok(cuttings);
    }
    [HttpGet]
    public async Task<IActionResult> GetCuttingColorById(long id)
    {
        var cutting = await cuttingRepo.GetCuttingColorByIdAsync(id);
        return Ok(cutting);
    }
    [HttpPost]
    public async Task<IActionResult> UpdateCuttingColor(long id, CuttingColorFormDto dto)
    {
        await cuttingRepo.UpdateCuttingColorAsync(id, dto);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> RemoveCuttingColor(long id)
    {
        await cuttingRepo.RemoveCuttingColorAsync(id);
        return Ok();
    }
    #endregion
}
