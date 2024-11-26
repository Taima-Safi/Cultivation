using Cultivation.Dto.Client;
using Cultivation.Repository.Client;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly IClientRepo clientRepo;

    public ClientController(IClientRepo clientRepo)
    {
        this.clientRepo = clientRepo;
    }
    [HttpPost]
    public async Task<IActionResult> Add(ClientFormDto dto)
    {
        var clientId = await clientRepo.AddAsync(dto);
        return Ok(clientId);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(bool? isLocal, string name, int pageSize = 10, int pageNum = 0)
    {
        var clients = await clientRepo.GetAllAsync(isLocal, name, pageSize, pageNum);
        return Ok(clients);
    }
    [HttpPost]
    public async Task<IActionResult> Update(long id, string phone, string codePhone, string name)
    {
        await clientRepo.UpdateAsync(id, phone, codePhone, name);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> Remove(long id)
    {
        await clientRepo.RemoveAsync(id);
        return Ok();
    }
}
