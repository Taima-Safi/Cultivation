using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Color;
using Cultivation.Dto.Cutting;
using Cultivation.Dto.CuttingLand;
using Cultivation.Dto.Fertilizer;
using Cultivation.Dto.FertilizerLand;
using Cultivation.Dto.Land;
using Cultivation.Repository.Base;
using Cultivation.Repository.CuttingLand;
using Cultivation.Repository.Fertilizer;
using Cultivation.Repository.Land;
using FourthPro.Dto.Common;
using FourthPro.Shared.Exception;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cultivation.Repository.FertilizerLand;

public class FertilizerLandRepo : IFertilizerLandRepo
{
    private readonly CultivationDbContext context;
    private readonly ICuttingLandRepo cuttingLandRepo;
    private readonly IFertilizerRepo fertilizerRepo;
    private readonly ILandRepo landRepo;
    private readonly IBaseRepo<FertilizerLandModel> baseRepo;

    public FertilizerLandRepo(CultivationDbContext context, ILandRepo landRepo, IFertilizerRepo fertilizerRepo, IBaseRepo<FertilizerLandModel> baseRepo, ICuttingLandRepo cuttingLandRepo)
    {
        this.context = context;
        this.landRepo = landRepo;
        this.fertilizerRepo = fertilizerRepo;
        this.baseRepo = baseRepo;
        this.cuttingLandRepo = cuttingLandRepo;
    }
    public async Task AddAsync(FertilizerLandFormDto dto)
    {
        if (!await landRepo.CheckIfExistByIdsAsync(dto.LandIds))
            throw new NotFoundException("One of lands not found..");

        var cuttingLandIds = await cuttingLandRepo.GetActiveCuttingLandIdsAsync(dto.LandIds);

        if (cuttingLandIds == null)
            throw new NotFoundException("not found..");

        if (!await fertilizerRepo.CheckIfExistByIdsAsync(dto.Mixes.Select(f => f.FertilizerId).ToList()))
            throw new NotFoundException("One of Fertilizers not found..");

        List<FertilizerLandModel> models = [];
        foreach (var cuttingLandId in cuttingLandIds)
        {
            models.AddRange(dto.Mixes.Select(f => new FertilizerLandModel
            {
                Date = dto.Date,
                Type = dto.Type,
                Quantity = f.Quantity,
                CuttingLandId = cuttingLandId,
                FertilizerId = f.FertilizerId,
            }).ToList());
        }
        await context.FertilizerLand.AddRangeAsync(models);
        await context.SaveChangesAsync();
    }
    public async Task<CommonResponseDto<List<GroupedFertilizerLandDto>>> GetAllAsync(DateTime? date, int pageSize, int pageNum)
    {
        var result = await context.FertilizerLand.Where(fl => (date.HasValue ? fl.Date.Date == date : fl.CuttingLand.IsActive) && fl.IsValid)
        .Include(fl => fl.Fertilizer).Include(fl => fl.CuttingLand).ThenInclude(l => l.Land).Include(fl => fl.CuttingLand).ThenInclude(l => l.CuttingColor)
        .OrderByDescending(fl => fl.CuttingLand.LandId)
            .ToListAsync();

        var x = result
            .GroupBy(fl => fl.Date)
            .OrderByDescending(group => group.Key)
            .Skip(pageNum * pageSize)
            .Take(pageSize)
            .Select(group => new GroupedFertilizerLandDto
            {
                Date = group.Key,  // The Date
                FertilizerLand = group.Select(fl => new FertilizerLandDto
                {
                    Id = fl.Id,
                    Type = fl.Type,
                    Quantity = fl.Quantity,
                    Fertilizer = new FertilizerDto
                    {
                        Id = fl.Fertilizer.Id,
                        NPK = fl.Fertilizer.NPK,
                        Title = fl.Fertilizer.Title,
                        PublicTitle = fl.Fertilizer.PublicTitle,
                        Description = fl.Fertilizer.Description,
                    },
                    CuttingLand = new CuttingLandDto
                    {
                        Id = fl.CuttingLand.Id,
                        Date = fl.CuttingLand.Date,
                        Quantity = fl.CuttingLand.Quantity,
                        Land = new LandDto
                        {
                            Id = fl.CuttingLand.Land.Id,
                            Size = fl.CuttingLand.Land.Size,
                            Title = fl.CuttingLand.Land.Title,
                            Location = fl.CuttingLand.Land.Location,
                            ParentId = fl.CuttingLand.Land.ParentId,
                        },
                        CuttingColor = new CuttingColorDto
                        {
                            Id = fl.CuttingLand.CuttingColor.Id,
                            Code = fl.CuttingLand.CuttingColor.Code
                        }
                    }
                }).ToList() // Collection of FertilizerLandDto for each Date group
            }).ToList();

        bool hasNextPage = false;
        if (x.Count > 0)
            hasNextPage = await baseRepo.CheckIfHasNextPageAsync(fl => fl.IsValid, pageSize, pageNum);

        return new CommonResponseDto<List<GroupedFertilizerLandDto>>(x, hasNextPage);
    }
    public async Task<List<LandDto>> GetLandsWhichNotUsedInDayAsync(DateTime? date)
    {
        var landModels = await context.Land.Where(l => !l.Children.Any() && l.IsValid).ToListAsync();

        var result = await context.FertilizerLand.Where(fl => (date.HasValue ? fl.Date.Date == date : fl.Date.Date == DateTime.UtcNow.Date) && fl.IsValid)
            .Include(fl => fl.Fertilizer).Include(fl => fl.CuttingLand).ThenInclude(fl => fl.Land).ToListAsync();

        List<LandModel> landsNotUsed = new();

        foreach (var land in landModels)
        {
            var isUsed = result.Where(l => l.CuttingLand.LandId == land.Id).Any();
            if (!isUsed)
                landsNotUsed.Add(land);
        }

        return landsNotUsed.Select(l => new LandDto
        {
            Id = l.Id,
            Size = l.Size,
            Title = l.Title,
            ParentId = l.ParentId,
            Location = l.Location,
        }).ToList();
    }
    public async Task<CommonResponseDto<List<FertilizerLandDto>>> GetFertilizersLandAsync(long landId, DateTime? from, DateTime? to, int pageSize, int pageNum)
    {
        if (!await landRepo.CheckIfExistAsync(landId))
            throw new NotFoundException("Land not found..");

        Expression<Func<FertilizerLandModel, bool>> expression = fl =>
            //fl.CuttingLand.IsActive && fl.CuttingLand.LandId == landId && (!from.HasValue || fl.Date.Date >= from)
            //&& (!to.HasValue || fl.Date.Date <= to) && fl.IsValid;
            fl.CuttingLand.LandId == landId && fl.IsValid &&
            (
                (!from.HasValue && !to.HasValue && fl.CuttingLand.IsActive) || // If both are null, check IsActive
                (from.HasValue && fl.Date.Date >= from) ||                     // If from has a value, check Date >= from
                (to.HasValue && fl.Date.Date <= to)                            // If to has a value, check Date <= to
            );
        var result = await context.FertilizerLand.Where(expression)
            .Skip(pageNum * pageSize)
            .Take(pageSize)
            .Select(fl => new FertilizerLandDto
            {
                Id = fl.Id,
                Date = fl.Date,
                Type = fl.Type,
                Quantity = fl.Quantity,
                Fertilizer = new FertilizerDto
                {
                    Id = fl.Fertilizer.Id,
                    NPK = fl.Fertilizer.NPK,
                    Title = fl.Fertilizer.Title,
                    PublicTitle = fl.Fertilizer.PublicTitle,
                    Description = fl.Fertilizer.Description,
                },
                CuttingLand = new CuttingLandDto
                {
                    Id = fl.CuttingLand.Id,
                    Date = fl.CuttingLand.Date,
                    Quantity = fl.CuttingLand.Quantity,
                    Land = new LandDto
                    {
                        Id = fl.CuttingLand.Land.Id,
                        Size = fl.CuttingLand.Land.Size,
                        Title = fl.CuttingLand.Land.Title,
                        Location = fl.CuttingLand.Land.Location,
                        ParentId = fl.CuttingLand.Land.ParentId,
                    },
                    CuttingColor = new CuttingColorDto
                    {
                        Id = fl.CuttingLand.CuttingColor.Id,
                        Code = fl.CuttingLand.CuttingColor.Code,
                        Color = new ColorDto
                        {
                            Id = fl.CuttingLand.CuttingColor.Color.Id,
                            Code = fl.CuttingLand.CuttingColor.Color.Code,
                            Title = fl.CuttingLand.CuttingColor.Color.Title,
                        },
                        Cutting = new CuttingDto
                        {
                            Id = fl.CuttingLand.CuttingColor.Cutting.Id,
                            Age = fl.CuttingLand.CuttingColor.Cutting.Age,
                            Type = fl.CuttingLand.CuttingColor.Cutting.Type,
                            Title = fl.CuttingLand.CuttingColor.Cutting.Title,
                        }
                    }
                }
            }).ToListAsync();

        bool hasNextPage = false;
        if (result.Count > 0)
            hasNextPage = await baseRepo.CheckIfHasNextPageAsync(fl => fl.IsValid, pageSize, pageNum);

        return new CommonResponseDto<List<FertilizerLandDto>>(result, hasNextPage);
    }
    public async Task<FertilizerLandDto> GetByIdAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Land not has this fertilizer..");

        return await context.FertilizerLand.Where(fl => fl.Id == id && fl.IsValid)
            .Select(fl => new FertilizerLandDto
            {
                Id = fl.Id,
                Date = fl.Date,
                Type = fl.Type,
                Quantity = fl.Quantity,
                Fertilizer = new FertilizerDto
                {
                    Id = fl.Fertilizer.Id,
                    NPK = fl.Fertilizer.NPK,
                    PublicTitle = fl.Fertilizer.PublicTitle
                },
                CuttingLand = new CuttingLandDto
                {
                    Id = fl.CuttingLand.Id,
                    Date = fl.CuttingLand.Date,
                    Quantity = fl.CuttingLand.Quantity,
                    Land = new LandDto
                    {
                        Id = fl.CuttingLand.Land.Id,
                        Size = fl.CuttingLand.Land.Size,
                        Title = fl.CuttingLand.Land.Title,
                        Location = fl.CuttingLand.Land.Location,
                        ParentId = fl.CuttingLand.Land.ParentId,
                    },
                    CuttingColor = new CuttingColorDto
                    {
                        Id = fl.CuttingLand.CuttingColor.Id,
                        Code = fl.CuttingLand.CuttingColor.Code,
                        Color = new ColorDto
                        {
                            Id = fl.CuttingLand.CuttingColor.Color.Id,
                            Code = fl.CuttingLand.CuttingColor.Color.Code,
                            Title = fl.CuttingLand.CuttingColor.Color.Title,
                        },
                        Cutting = new CuttingDto
                        {
                            Id = fl.CuttingLand.CuttingColor.Cutting.Id,
                            Age = fl.CuttingLand.CuttingColor.Cutting.Age,
                            Type = fl.CuttingLand.CuttingColor.Cutting.Type,
                            Title = fl.CuttingLand.CuttingColor.Cutting.Title,
                        }
                    }
                }
            }).FirstOrDefaultAsync();
    }
    public async Task UpdateAsync(long id, UpdateFertilizerLandDto dto)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Land not has this fertilizer..");

        if (!await landRepo.CheckIfExistAsync(dto.LandId))
            throw new NotFoundException("Land not found..");

        if (!await fertilizerRepo.CheckIfExistAsync(dto.FertilizerId))
            throw new NotFoundException("Fertilizer not has this fertilizer..");

        var cuttingLandId = await cuttingLandRepo.GetCuttingLandIdAsync(dto.LandId);

        if (cuttingLandId == 0)
            throw new NotFoundException("Land does not has cuttings");

        await context.FertilizerLand.Where(fl => fl.Id == id && fl.IsValid).ExecuteUpdateAsync(fl => fl.SetProperty(fl => fl.Type, dto.Type)
        .SetProperty(fl => fl.Quantity, dto.Quantity).SetProperty(fl => fl.Date, dto.Date).SetProperty(fl => fl.FertilizerId, dto.FertilizerId)
        .SetProperty(fl => fl.CuttingLandId, cuttingLandId));
    }
    public async Task RemoveAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Land not has this fertilizer..");

        await context.FertilizerLand.Where(fl => fl.Id == id && fl.IsValid).ExecuteUpdateAsync(fl => fl.SetProperty(fl => fl.IsValid, false));
    }

    public async Task<bool> CheckIfExistAsync(long id)
        => await context.FertilizerLand.Where(fl => fl.Id == id && fl.IsValid).AnyAsync();
}