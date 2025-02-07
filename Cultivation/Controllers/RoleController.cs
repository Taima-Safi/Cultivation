using Cultivation.Repository.Role;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepo roleRepo;

        public RoleController(IRoleRepo roleRepo)
        {
            this.roleRepo = roleRepo;
        }
        [HttpPost]
        public async Task<IActionResult> Add(string title)
        {
            var roleId = await roleRepo.AddAsync(title);
            return Ok(roleId);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(string title)
        {
            var roles = await roleRepo.GetAllAsync(title);
            return Ok(roles);
        }
        [HttpGet]
        public async Task<IActionResult> GetById(long id)
        {
            var role = await roleRepo.GetByIdAsync(id);
            return Ok(role);
        }
        [HttpPost]
        public async Task<IActionResult> Update(long id, string title)
        {
            await roleRepo.UpdateAsync(id, title);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Remove(long id)
        {
            await roleRepo.RemoveAsync(id);
            return Ok();
        }
    }
}
