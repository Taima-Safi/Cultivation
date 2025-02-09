using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Role;
using Cultivation.Repository.Base;
using FourthPro.Shared.Exception;
using Microsoft.EntityFrameworkCore;

namespace Cultivation.Repository.Role;

public class RoleRepo : IRoleRepo
{
    private readonly CultivationDbContext context;
    private readonly IBaseRepo<RoleModel> baseRepo;

    public RoleRepo(CultivationDbContext context, IBaseRepo<RoleModel> baseRepo)
    {
        this.context = context;
        this.baseRepo = baseRepo;
    }
    public async Task<long> AddAsync(RoleFormDto dto)
    {
        var role = await context.Role.AddAsync(new RoleModel
        {
            Title = dto.Title,
            FullAccess = dto.FullAccess
        });
        await context.SaveChangesAsync();
        return role.Entity.Id;
    }
    public async Task<List<RoleDto>> GetAllAsync(string title)
    {
        var x = await context.Role.Where(c => (string.IsNullOrEmpty(title) || c.Title.Contains(title))
        && c.IsValid)
            .Select(c => new RoleDto
            {
                Id = c.Id,
                Title = c.Title,
                FullAccess = c.FullAccess
            }).ToListAsync();

        return x;
    }
    public async Task<RoleDto> GetByIdAsync(long id)
    {
        if (!await baseRepo.CheckIfExistAsync(x => x.Id == id && x.IsValid))
            throw new NotFoundException("Role not found..");

        return await context.Role.Where(c => c.Id == id && c.IsValid).Select(c => new RoleDto
        {
            Id = c.Id,
            Title = c.Title,
            FullAccess = c.FullAccess
        }).FirstOrDefaultAsync();
    }
    public async Task UpdateAsync(long id, RoleFormDto dto)
    {
        if (!await baseRepo.CheckIfExistAsync(x => x.Id == id && x.IsValid))
            throw new NotFoundException("Role not found..");

        await context.Role.Where(c => c.Id == id && c.IsValid).ExecuteUpdateAsync(c => c.SetProperty(c => c.Title, dto.Title).SetProperty(c => c.FullAccess, dto.FullAccess));
    }

    public async Task RemoveAsync(long id)
    {
        if (!await baseRepo.CheckIfExistAsync(x => x.Id == id && x.IsValid))
            throw new NotFoundException("Role not found..");

        await context.Role.Where(c => c.Id == id && c.IsValid).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsValid, false));
    }
}
