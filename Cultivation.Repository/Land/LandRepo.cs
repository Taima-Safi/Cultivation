using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Land;
using FourthPro.Shared.Exception;
using Microsoft.EntityFrameworkCore;

namespace Cultivation.Repository.Land;

public class LandRepo : ILandRepo
{
    private readonly CultivationDbContext context;

    public LandRepo(CultivationDbContext context)
    {
        this.context = context;
    }
    public async Task<long> AddAsync(LandFormDto dto)
    {
        if (dto.ParentId.HasValue)
            if (!await CheckIfExistAsync(dto.ParentId.Value))
                throw new NotFoundException("Parent land Not Found..");

        var land = await context.Land.AddAsync(new LandModel
        {
            Size = dto.Size,
            Title = dto.Title,
            ParentId = dto.ParentId
        });
        await context.SaveChangesAsync();
        return land.Entity.Id;
    }
    public async Task<List<LandDto>> GetAllAsync()
    {
        var landModels = await context.Land.Where(l => l.IsValid)
            .Select(l => new LandDto
            {
                Id = l.Id,
                Size = l.Size,
                Title = l.Title,
                ParentId = l.Parent.Id,
                Children = l.Children.Select(l => new LandDto
                {
                    Id = l.Id,
                    Size = l.Size,
                    Title = l.Title,
                    ParentId = l.Parent.Id,
                }).ToList()
            }).ToListAsync();
        return landModels;
    }
    public async Task<List<LandDto>> GetLandRecursion()
    {
        var models = await GetAllAsync();
        var parents = models.Where(l => !l.ParentId.HasValue).ToList();
        var result = new List<LandDto>();
        foreach (var parent in parents)
        {
            result.Add(GetChildrenAsync(parent, models));
        }
        return result;
    }
    private LandDto GetChildrenAsync(LandDto parent, List<LandDto> allLands)
    {
        var children = allLands.Where(a => a.ParentId == parent.Id).ToList();
        parent.Children = children;
        foreach (var child in children)
        {
            GetChildrenAsync(child, allLands);
        }

        return parent;
    }
    public async Task<bool> CheckIfExistAsync(long id)
        => await context.Land.Where(l => l.Id == id && l.IsValid).AnyAsync();
}
