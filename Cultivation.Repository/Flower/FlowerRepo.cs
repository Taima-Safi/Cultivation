using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Color;
using Cultivation.Dto.Cutting;
using Cultivation.Dto.CuttingLand;
using Cultivation.Dto.Flower;
using Cultivation.Dto.Land;
using Cultivation.Repository.Base;
using FourthPro.Dto.Common;
using FourthPro.Shared.Exception;
using Microsoft.EntityFrameworkCore;

namespace Cultivation.Repository.Flower;

public class FlowerRepo : IFlowerRepo
{
    private readonly CultivationDbContext context;
    private readonly IBaseRepo<FertilizerLandModel> baseRepo;

    public FlowerRepo(CultivationDbContext context, IBaseRepo<FertilizerLandModel> baseRepo)
    {
        this.context = context;
        this.baseRepo = baseRepo;
    }
    public async Task<long> AddAsync(int count, string note, DateTime date, long cuttingLandId)
    {
        if (!await context.CuttingLand.Where(c => c.Id == cuttingLandId && c.IsValid).AnyAsync())
            throw new NotFoundException("Cuttings not found..");

        var x = await context.Flower.AddAsync(new FlowerModel
        {
            Date = date,
            Note = note,
            Count = count,
            CuttingLandId = cuttingLandId
        });
        await context.SaveChangesAsync();
        return x.Entity.Id;
    }
    public async Task<CommonResponseDto<List<FlowerDto>>> GetAllAsync(DateTime? from, DateTime? to, long? cuttingLandId, string cuttingTitle
        , string cuttingColorCode, string colorTitle, int pageSize, int pageNum)
    {
        var x = await context.Flower.Where(f => (!from.HasValue || f.Date.Date >= from)
        && (!to.HasValue || f.Date.Date <= to)
        && (!cuttingLandId.HasValue || f.CuttingLandId == cuttingLandId)
        && (string.IsNullOrEmpty(colorTitle) || f.CuttingLand.CuttingColor.Color.Title == colorTitle)
        && (string.IsNullOrEmpty(cuttingColorCode) || f.CuttingLand.CuttingColor.Code == cuttingColorCode)
        && (string.IsNullOrEmpty(cuttingTitle) || f.CuttingLand.CuttingColor.Cutting.Title == cuttingTitle)
        // && (!colorId.HasValue || f.CuttingLand.CuttingColor.ColorId == colorId)
        && f.IsValid)
            .OrderByDescending(f => f.Date)
            .OrderByDescending(f => f.CuttingLandId)
            .Skip(pageNum * pageSize)
            .Take(pageSize)
            .Select(c => new FlowerDto
            {
                Id = c.Id,
                Date = c.Date,
                Note = c.Note,
                Count = c.Count,
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

        bool hasNextPage = false;
        if (x.Count > 0)
            hasNextPage = await baseRepo.CheckIfHasNextPageAsync(fl => fl.IsValid, pageSize, pageNum);

        return new CommonResponseDto<List<FlowerDto>>(x, hasNextPage);
    }
    public async Task<FlowerDto> GetByIdAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Flowers not found..");

        return await context.Flower.Where(c => c.Id == id && c.IsValid).Select(c => new FlowerDto
        {
            Id = c.Id,
            Date = c.Date,
            Note = c.Note,
            Count = c.Count,
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
    public async Task UpdateAsync(long id, string note, int count, DateTime date)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Flowers not found..");

        await context.Flower.Where(c => c.Id == id && c.IsValid).ExecuteUpdateAsync(c => c.SetProperty(c => c.Date, date)
        .SetProperty(c => c.Note, note).SetProperty(c => c.Count, count));
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
