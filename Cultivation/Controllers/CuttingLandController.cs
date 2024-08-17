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
    //[HttpPost]
    //public async Task<IActionResult> Add(CuttingLandFormDto dto)
    //{
    //    var id = await cuttingLandRepo.AddAsync(dto);
    //    return Ok(id);
    //}
    //[HttpGet]
    //public async Task<IActionResult> GetAll(DateTime? date, long? quantity)
    //{
    //    var cuttingLands = await cuttingLandRepo.GetAllAsync(date, quantity);
    //    return Ok(cuttingLands);
    //}
    //[HttpGet]
    //public async Task<IActionResult> GetById(long id)
    //{
    //    var cuttingLand = await cuttingLandRepo.GetByIdAsync(id);
    //    return Ok(cuttingLand);
    //}
    //[HttpPost]
    //public async Task<IActionResult> Update(long id, string title, string type, int age)
    //{
    //    await cuttingLandRepo.UpdateAsync(id, title, type, age);
    //    return Ok();
    //}
    //[HttpDelete]
    //public async Task<IActionResult> Remove(long id)
    //{
    //    await cuttingLandRepo.RemoveAsync(id);
    //    return Ok();
    //}
}
