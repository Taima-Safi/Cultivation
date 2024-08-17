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
            Location = dto.Location,
            ParentId = dto.ParentId
        });
        await context.SaveChangesAsync();
        return land.Entity.Id;
    }
    public async Task<List<LandDto>> GetAllAsync(string title, double? size)
    {
        var landModels = await context.Land.Where(l => (string.IsNullOrEmpty(title) || l.Title.Contains(title))
        && (!size.HasValue || l.Size == size)
        && l.IsValid)
            .Select(l => new LandDto
            {
                Id = l.Id,
                Size = l.Size,
                Title = l.Title,
                Location = l.Location,
                ParentId = l.Parent.Id,
                Children = l.Children.Select(l => new LandDto
                {
                    Id = l.Id,
                    Size = l.Size,
                    Title = l.Title,
                    Location = l.Location,
                    ParentId = l.Parent.Id,
                }).ToList()
            }).ToListAsync();
        var parents = landModels.Where(l => !l.ParentId.HasValue).ToList();
        var result = new List<LandDto>();
        foreach (var parent in parents)
        {
            result.Add(GetChildrenAsync(parent, landModels));
        }
        return result;
    }

    public async Task<LandDto> GetByIdAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Land not found..");

        var landModels = await context.Land.Where(l => l.IsValid)
            .Select(l => new LandDto
            {
                Id = l.Id,
                Size = l.Size,
                Title = l.Title,
                Location = l.Location,
                ParentId = l.Parent.Id,
                Children = l.Children.Select(l => new LandDto
                {
                    Id = l.Id,
                    Size = l.Size,
                    Title = l.Title,
                    Location = l.Location,
                    ParentId = l.Parent.Id,
                }).ToList()
            }).ToListAsync();

        var parent = landModels.Where(l => l.Id == id).FirstOrDefault();

        var result = GetChildrenAsync(parent, landModels);
        return result;
    }
    //public async Task<List<LandDto>> GetLandRecursion()
    //{
    //    var models = await GetAllAsync();
    //    var parents = models.Where(l => !l.ParentId.HasValue).ToList();
    //    var result = new List<LandDto>();
    //    foreach (var parent in parents)
    //    {
    //        result.Add(GetChildrenAsync(parent, models));
    //    }
    //    return result;
    //}
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
