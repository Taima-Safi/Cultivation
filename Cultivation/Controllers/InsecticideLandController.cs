using Cultivation.Dto.InsecticideLand;
using Cultivation.Repository.InsecticideLand;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]

public class InsecticideLandController : ControllerBase
{
    private readonly IInsecticideLandRepo insecticideLandRepo;

    public InsecticideLandController(IInsecticideLandRepo insecticideLandRepo)
    {
        this.insecticideLandRepo = insecticideLandRepo;
    }

    [HttpPost]
    public async Task<IActionResult> Add(InsecticideLandFormDto dto)
    {
        await insecticideLandRepo.AddAsync(dto);
        return Ok();
    }
    [HttpPost]
    public async Task<IActionResult> ExportExcel(long landId, DateTime? from, DateTime? to, string fileName = "ExcelFile")
    {
        var excel = await insecticideLandRepo.ExportExcelAsync(landId, from, to, fileName);
        return File(excel.stream, excel.file.ContentType, excel.file.FileName);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(/*DateTime? date, */string note, double? liter, double? quantity, DateTime? from, DateTime? to,
        long? landId, long? insecticideId, int pageSize = 10, int pageNum = 0)
    {
        var result = await insecticideLandRepo.GetAllAsync(/*date,*/ note, liter, quantity, from, to, landId, insecticideId, pageSize, pageNum);
        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetLandsWhichNotUsedInDay(DateTime? date)
    {
        var result = await insecticideLandRepo.GetLandsWhichNotUsedInDayAsync(date);
        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await insecticideLandRepo.GetByIdAsync(id);
        return Ok(result);
    }
    [HttpPost]
    public async Task<IActionResult> Update(long id, UpdateInsecticideLandFormDto dto)
    {
        await insecticideLandRepo.UpdateAsync(id, dto);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> Remove(long id)
    {
        await insecticideLandRepo.RemoveAsync(id);
        return Ok();
    }
}
