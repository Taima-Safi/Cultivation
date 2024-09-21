using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Color;
using Cultivation.Dto.Cutting;
using Cultivation.Dto.CuttingLand;
using Cultivation.Dto.Flower;
using Cultivation.Dto.Land;
using FourthPro.Shared.Exception;
using Microsoft.EntityFrameworkCore;

namespace Cultivation.Repository.Flower;

public class FlowerRepo : IFlowerRepo
{
    private readonly CultivationDbContext context;

    public FlowerRepo(CultivationDbContext context)
    {
        this.context = context;
    }
    public async Task<long> AddAsync(int count, DateTime date, long cuttingLandId)
    {
        if (!await context.CuttingLand.Where(c => c.Id == cuttingLandId && c.IsValid).AnyAsync())
            throw new NotFoundException("Cuttings not found..");

        var x = await context.Flower.AddAsync(new FlowerModel
        {
            Date = date,
            Count = count,
            CuttingLandId = cuttingLandId
        });
        await context.SaveChangesAsync();
        return x.Entity.Id;
    }
    public async Task<List<FlowerDto>> GetAllAsync(DateTime? date, long? cuttingLandId)
    {
        return await context.Flower.Where(c => (!date.HasValue || c.Date == date)
        && (!cuttingLandId.HasValue || c.CuttingLandId == cuttingLandId)
        && c.IsValid).Select(c => new FlowerDto
        {
            Id = c.Id,
            Count = c.Count,
            Date = c.Date,
            CuttingLand = new CuttingLandDto
            {
                Id = c.CuttingLand.Id,
                Land = new LandDto
                {
                    Id = c.CuttingLand.Land.Id,
                    Size = c.CuttingLand.Land.Size,
                    Title = c.CuttingLand.Land.Title,
                    ParentId = c.CuttingLand.Land.ParentId,
                    Location = c.CuttingLand.Land.Location,
                },
                CuttingColor = new CuttingColorDto
                {
                    Id = c.CuttingLand.CuttingColor.Id,
                    Code = c.CuttingLand.CuttingColor.Code,
                    Color = new ColorDto
                    {
                        Id = c.CuttingLand.CuttingColor.Color.Id,
                        Title = c.CuttingLand.CuttingColor.Color.Title,
                    },
                    Cutting = new CuttingDto
                    {
                        Id = c.CuttingLand.CuttingColor.Cutting.Id,
                        Type = c.CuttingLand.CuttingColor.Cutting.Type,
                        Title = c.CuttingLand.CuttingColor.Cutting.Title,
                    }
                }
            }
        }).ToListAsync();
    }
    public async Task<FlowerDto> GetByIdAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Flowers not found..");

        return await context.Flower.Where(c => c.Id == id && c.IsValid).Select(c => new FlowerDto
        {
            Id = c.Id,
            Count = c.Count,
            Date = c.Date,
            CuttingLand = new CuttingLandDto
            {
                Id = c.CuttingLand.Id,
                Date = c.CuttingLand.Date,
                Quantity = c.CuttingLand.Quantity,
                Land = new LandDto
                {
                    Id = c.CuttingLand.Land.Id,
                    Size = c.CuttingLand.Land.Size,
                    Title = c.CuttingLand.Land.Title,
                    ParentId = c.CuttingLand.Land.ParentId,
                    Location = c.CuttingLand.Land.Location,
                },
                CuttingColor = new CuttingColorDto
                {
                    Id = c.CuttingLand.CuttingColor.Id,
                    Code = c.CuttingLand.CuttingColor.Code,
                    Color = new ColorDto
                    {
                        Id = c.CuttingLand.CuttingColor.Color.Id,
                        Code = c.CuttingLand.CuttingColor.Color.Code,
                        Title = c.CuttingLand.CuttingColor.Color.Title,
                    },
                    Cutting = new CuttingDto
                    {
                        Id = c.CuttingLand.CuttingColor.Cutting.Id,
                        Type = c.CuttingLand.CuttingColor.Cutting.Type,
                        Title = c.CuttingLand.CuttingColor.Cutting.Title,
                    }
                }
            }
        }).FirstOrDefaultAsync();
    }
    public async Task UpdateAsync(long id, int count, DateTime date)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Flowers not found..");

        await context.Flower.Where(c => c.Id == id && c.IsValid).ExecuteUpdateAsync(c => c.SetProperty(c => c.Date, date).SetProperty(c => c.Count, count));
    }

    public async Task RemoveAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Flowers not found..");

        await context.Flower.Where(c => c.Id == id && c.IsValid).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsValid, false));
    }

    public async Task<bool> CheckIfExistAsync(long id)
     => await context.Flower.Where(c => c.Id == id && c.IsValid).AnyAsync();
}
