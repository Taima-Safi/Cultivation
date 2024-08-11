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
    public async Task<IActionResult> GetAll()
        => Ok(await landRepo.GetLandRecursion());
}
