using Cultivation.Dto.User;
using Cultivation.Repository.User;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepo userRepo;

    public UserController(IUserRepo userRepo)
    {
        this.userRepo = userRepo;
    }
    [HttpPost]
    public async Task<IActionResult> Add(UserFormDto dto)
    {
        var id = await userRepo.AddAsync(dto);
        return Ok(id);
    }
    [HttpPost]
    public async Task<IActionResult> Update(UserFormDto dto, long id)

    {
        await userRepo.UpdateAsync(dto, id);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> Remove(long id)
    {
        await userRepo.RemoveAsync(id);
        return Ok();
    }
}
