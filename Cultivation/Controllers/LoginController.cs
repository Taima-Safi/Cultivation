using Cultivation.Dto.User;
using Cultivation.Repository.Token;
using Cultivation.Repository.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IUserRepo userRepo;
    private readonly ITokenRepo tokenRepo;

    public LoginController(IUserRepo userRepo, ITokenRepo tokenRepo)
    {
        this.userRepo = userRepo;
        this.tokenRepo = tokenRepo;
    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var userDto = await userRepo.LoginAsync(dto);
        userDto.Token = await tokenRepo.CreateAsync(userDto.Id, userDto.Type.ToString());

        return Ok(userDto);
    }
    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> RefreshToken(string RefreshToken)
    {
        var newToken = await tokenRepo.RefreshTokenAsync(RefreshToken);
        return Ok(newToken);
    }
}
