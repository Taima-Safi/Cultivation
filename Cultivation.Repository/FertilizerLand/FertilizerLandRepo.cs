﻿using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Color;
using Cultivation.Dto.Common;
using Cultivation.Dto.Cutting;
using Cultivation.Dto.CuttingLand;
using Cultivation.Dto.Fertilizer;
using Cultivation.Dto.FertilizerLand;
using Cultivation.Dto.Land;
using Cultivation.Repository.Base;
using Cultivation.Repository.CuttingLand;
using Cultivation.Repository.DataBase;
using Cultivation.Repository.Fertilizer;
using Cultivation.Repository.File;
using Cultivation.Repository.Land;
using Cultivation.Shared.Enum;
using Cultivation.Shared.Exception;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cultivation.Repository.FertilizerLand;

public class FertilizerLandRepo : IFertilizerLandRepo
{
    private readonly IDbRepo dbRepo;
    private readonly CultivationDbContext context;
    private readonly ICuttingLandRepo cuttingLandRepo;
    private readonly IFertilizerRepo fertilizerRepo;
    private readonly ILandRepo landRepo;
    private readonly IFileRepo<FertilizerExportToExcelDto> fileRepo;
    private readonly IBaseRepo<FertilizerLandModel> baseRepo;
    private readonly IBaseRepo<FertilizerMixModel> mixBaseRepo;
    private readonly IBaseRepo<CuttingLandModel> cuttingLandBaseRepo;
    private readonly IBaseRepo<LandModel> landBaseRepo;

    public FertilizerLandRepo(CultivationDbContext context, ILandRepo landRepo, IFertilizerRepo fertilizerRepo, IBaseRepo<FertilizerLandModel> baseRepo,
        ICuttingLandRepo cuttingLandRepo, IFileRepo<FertilizerExportToExcelDto> fileRepo, IBaseRepo<FertilizerMixModel> mixRepo, IBaseRepo<CuttingLandModel> cuttingLandBaseRepo, IBaseRepo<LandModel> landBaseRepo, IDbRepo dbRepo)
    {
        this.context = context;
        this.landRepo = landRepo;
        this.fertilizerRepo = fertilizerRepo;
        this.baseRepo = baseRepo;
        this.cuttingLandRepo = cuttingLandRepo;
        this.fileRepo = fileRepo;
        this.mixBaseRepo = mixRepo;
        this.cuttingLandBaseRepo = cuttingLandBaseRepo;
        this.landBaseRepo = landBaseRepo;
        this.dbRepo = dbRepo;
    }

    public async Task<(FormFile file, MemoryStream stream)> ExportExcelAsync(long landId, DateTime? from, DateTime? to, string fileName)
    {
        var result = await GetFertilizersLandAsync(landId, from, to);
        var toExport = result.Data.Select(x => new FertilizerExportToExcelDto
        {
            Type = x.Type.ToString(),
            Date = x.Date.ToShortDateString(),
            Quantity = x.Quantity.ToString(),
            NPK = x.Fertilizer.NPK,
            Title = x.Fertilizer.Title,
            Description = x.Fertilizer.Description,
            PublicTitle = x.Fertilizer.PublicTitle,
        }).ToList();

        var excel = fileRepo.ExportToExcel(ExportType.LandFertilizers, fileName, new(), /*filtersPropertiesNames,*/ toExport);
        return excel;
    }
    public async Task AddAsync(FertilizerLandFormDto dto)
    {
        //if (!await landRepo.CheckIfExistByIdsAsync(dto.LandIds))
        //    throw new NotFoundException("One of lands not found..");

        //var cuttingLandIds = await cuttingLandRepo.GetActiveCuttingLandIdsAsync(dto.LandIds);

        //if (cuttingLandIds == null)
        //  throw new NotFoundException("not found..");


        if (!await landRepo.CheckIfExistByIdsAsync(dto.LandIds))
            throw new NotFoundException("One of Lands not found..");


        if (!await fertilizerRepo.CheckIfExistByIdsAsync(dto.Mixes.Select(f => f.FertilizerId).ToList()))
            throw new NotFoundException("One of Fertilizers not found..");

        List<FertilizerLandModel> models = [];
        foreach (var landId in dto.LandIds)
        {
            models.AddRange(dto.Mixes.Select(f => new FertilizerLandModel
            {
                Date = dto.Date,
                Type = dto.Type,
                LandId = landId,
                Quantity = f.Quantity,
                FertilizerId = f.FertilizerId,
            }).ToList());
        }
        await context.FertilizerLand.AddRangeAsync(models);
        await context.SaveChangesAsync();
    }
    public async Task<CommonResponseDto<List<GroupedFertilizerLandDto>>> GetAllAsync(long? landId, DateTime? from, DateTime? to, int pageSize, int pageNum)
    {
        var result = await context.FertilizerLand.Where(fl =>
         (!landId.HasValue || fl.Land.Id == landId) &&
            (
                (!from.HasValue && !to.HasValue && fl.Land.CuttingLands.Any(c => c.IsActive)) || // If both are null, check IsActive
                (from.HasValue && fl.Date.Date >= from) ||                     // If from has a value, check Date >= from
                (to.HasValue && fl.Date.Date <= to)                            // If to has a value, check Date <= to
            )
        && fl.IsValid)
        .Include(fl => fl.Fertilizer).Include(l => l.Land).ThenInclude(fl => fl.CuttingLands).ThenInclude(l => l.CuttingColor)
        .OrderByDescending(fl => fl.LandId)
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
                    Land = new LandDto
                    {
                        Id = fl.Land.Id,
                        Size = fl.Land.Size,
                        Title = fl.Land.Title,
                        ParentId = fl.Land.ParentId,
                        Location = fl.Land.Location,
                        CuttingLands = fl.Land.CuttingLands.Where(cl => cl.IsValid).Select(cl => new CuttingLandDto
                        {
                            Id = cl.Id,
                            Date = cl.Date,
                            Quantity = cl.Quantity,
                            IsActive = cl.IsActive,
                            CuttingColor = new CuttingColorDto
                            {
                                Id = cl.CuttingColor.Id,
                                Code = cl.CuttingColor.Code
                            },
                        }).ToList()
                    },
                    //CuttingLand = new CuttingLandDto
                    //{
                    //    Id = fl.CuttingLand.Id,
                    //    Date = fl.CuttingLand.Date,
                    //    Quantity = fl.CuttingLand.Quantity,
                    //    Land = new LandDto
                    //    {
                    //        Id = fl.CuttingLand.Land.Id,
                    //        Size = fl.CuttingLand.Land.Size,
                    //        Title = fl.CuttingLand.Land.Title,
                    //        Location = fl.CuttingLand.Land.Location,
                    //        ParentId = fl.CuttingLand.Land.ParentId,
                    //    },
                    //    CuttingColor = new CuttingColorDto
                    //    {
                    //        Id = fl.CuttingLand.CuttingColor.Id,
                    //        Code = fl.CuttingLand.CuttingColor.Code
                    //    }
                    //}
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
            .Include(fl => fl.Fertilizer).Include(fl => fl.Land).ToListAsync();

        List<LandModel> landsNotUsed = new();

        foreach (var land in landModels)
        {
            var isUsed = result.Where(l => l.LandId == land.Id).Any();
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
    public async Task<CommonResponseDto<List<FertilizerLandModel>>> GetFertilizersLandAsync(long landId, DateTime? from, DateTime? to)
    {
        if (!await landRepo.CheckIfExistAsync(landId))
            throw new NotFoundException("Land not found..");

        Expression<Func<FertilizerLandModel, bool>> expression = fl =>
            //fl.CuttingLand.IsActive && fl.CuttingLand.LandId == landId && (!from.HasValue || fl.Date.Date >= from)
            //&& (!to.HasValue || fl.Date.Date <= to) && fl.IsValid;
            fl.LandId == landId && fl.IsValid &&
            (
                (!from.HasValue && !to.HasValue && fl.Land.CuttingLands.Any(cl => cl.IsActive)) || // If both are null, check IsActive
                (from.HasValue && fl.Date.Date >= from) ||                     // If from has a value, check Date >= from
                (to.HasValue && fl.Date.Date <= to)                            // If to has a value, check Date <= to
            );
        var result = await context.FertilizerLand.Where(expression).Include(fl => fl.Fertilizer).ToListAsync();

        return new CommonResponseDto<List<FertilizerLandModel>>(result);
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
                Land = new LandDto
                {
                    Id = fl.Land.Id,
                    Size = fl.Land.Size,
                    Title = fl.Land.Title,
                    ParentId = fl.Land.ParentId,
                    Location = fl.Land.Location,
                    CuttingLands = fl.Land.CuttingLands.Where(cl => cl.IsValid).Select(cl => new CuttingLandDto
                    {
                        Id = cl.Id,
                        Date = cl.Date,
                        Quantity = cl.Quantity,
                        IsActive = cl.IsActive,
                        CuttingColor = new CuttingColorDto
                        {
                            Id = cl.CuttingColor.Id,
                            Code = cl.CuttingColor.Code,
                            Color = new ColorDto
                            {
                                Id = cl.CuttingColor.Color.Id,
                                Code = cl.CuttingColor.Color.Code,
                                Title = cl.CuttingColor.Color.Title,
                            },
                            Cutting = new CuttingDto
                            {
                                Id = cl.CuttingColor.Cutting.Id,
                                Age = cl.CuttingColor.Cutting.Age,
                                Type = cl.CuttingColor.Cutting.Type,
                                Title = cl.CuttingColor.Cutting.Title,
                            }
                        },
                    }).ToList()
                },
                //CuttingLand = new CuttingLandDto
                //{
                //    Id = fl.CuttingLand.Id,
                //    Date = fl.CuttingLand.Date,
                //    Quantity = fl.CuttingLand.Quantity,
                //    Land = new LandDto
                //    {
                //        Id = fl.CuttingLand.Land.Id,
                //        Size = fl.CuttingLand.Land.Size,
                //        Title = fl.CuttingLand.Land.Title,
                //        Location = fl.CuttingLand.Land.Location,
                //        ParentId = fl.CuttingLand.Land.ParentId,
                //    },
                //    CuttingColor = new CuttingColorDto
                //    {
                //        Id = fl.CuttingLand.CuttingColor.Id,
                //        Code = fl.CuttingLand.CuttingColor.Code,
                //        Color = new ColorDto
                //        {
                //            Id = fl.CuttingLand.CuttingColor.Color.Id,
                //            Code = fl.CuttingLand.CuttingColor.Color.Code,
                //            Title = fl.CuttingLand.CuttingColor.Color.Title,
                //        },
                //        Cutting = new CuttingDto
                //        {
                //            Id = fl.CuttingLand.CuttingColor.Cutting.Id,
                //            Age = fl.CuttingLand.CuttingColor.Cutting.Age,
                //            Type = fl.CuttingLand.CuttingColor.Cutting.Type,
                //            Title = fl.CuttingLand.CuttingColor.Cutting.Title,
                //        }
                //    }
                //},
            }).FirstOrDefaultAsync();
    }
    public async Task UpdateAsync(long id, UpdateFertilizerLandDto dto)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Land not has this fertilizer..");

        if (!await landRepo.CheckIfExistAsync(dto.LandId))
            throw new NotFoundException("land not found..");

        if (!await fertilizerRepo.CheckIfExistAsync(dto.FertilizerId))
            throw new NotFoundException("Fertilizer not has this fertilizer..");


        await context.FertilizerLand.Where(fl => fl.Id == id && fl.IsValid).ExecuteUpdateAsync(fl => fl.SetProperty(fl => fl.Type, dto.Type)
        .SetProperty(fl => fl.Quantity, dto.Quantity).SetProperty(fl => fl.Date, dto.Date).SetProperty(fl => fl.FertilizerId, dto.FertilizerId)
        .SetProperty(fl => fl.LandId, dto.LandId));
    }
    public async Task RemoveAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Land not has this fertilizer..");

        await context.FertilizerLand.Where(fl => fl.Id == id && fl.IsValid).ExecuteUpdateAsync(fl => fl.SetProperty(fl => fl.IsValid, false));
    }

    public async Task<bool> CheckIfExistAsync(long id)
        => await context.FertilizerLand.Where(fl => fl.Id == id && fl.IsValid).AnyAsync();

    #region MixLand

    public async Task AddMixLandAsync(long mixId, long landId)
    {
        try
        {
            await dbRepo.BeginTransactionAsync();

            if (!await mixBaseRepo.CheckIfExistAsync(m => m.Id == mixId && m.IsValid))
                throw new NotFoundException("mix not found..");

            if (!await landBaseRepo.CheckIfExistAsync(m => m.Id == landId && m.IsValid))
                throw new NotFoundException("land not found..");

            //Dictionary<long, double> dic = await context.FertilizerMixDetail.Where(x => x.FertilizerMixId == mixId && x.IsValid)
            //    .ToDictionaryAsync(x => x.FertilizerId, x => x.Quantity);

            //foreach (var item in dic)
            //    await fertilizerRepo.UpdateStoreAsync(item.Key, item.Value, DateTime.UtcNow, false);

            await context.FertilizerMixLand.AddAsync(new FertilizerMixLandModel
            {
                LandId = landId,
                Date = DateTime.UtcNow,
                FertilizerMixId = mixId,
            });


            await dbRepo.SaveChangesAsync();
            await dbRepo.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await dbRepo.RollbackTransactionAsync();
            throw;
        }
    }
    public async Task AddMixLandsAsync(long mixId, List<long> landIds)
    {
        try
        {
            await dbRepo.BeginTransactionAsync();

            if (landIds == null || !landIds.Any())
                throw new ArgumentException("Land IDs cannot be empty");

            if (!await mixBaseRepo.CheckIfExistAsync(m => m.Id == mixId && m.IsValid))
                throw new NotFoundException("mix not found..");

            if (!await landBaseRepo.CheckIfExistAsync(m => landIds.Contains(m.Id) && m.IsValid))
                throw new NotFoundException("one of lands not found..");

            var mixDonum = await context.FertilizerApplicableMix.Where(x => x.FertilizerMixId == mixId && x.CurrentDonumCount > 0 && x.IsValid).FirstOrDefaultAsync();
            if (mixDonum == null)
                throw new NotFoundException("No available mix with positive donum count");

            var landSizes = await context.Land
                .Where(l => landIds.Contains(l.Id))
                .Select(l => new { l.Id, l.Size })
                .ToListAsync();

            double totalLandDonum = landSizes.Sum(x => x.Size);
            if (totalLandDonum > mixDonum.CurrentDonumCount)
                throw new NotFoundException("You do not have enough mix ");

            mixDonum.CurrentDonumCount -= totalLandDonum;

            var models = landIds.Select(id => new FertilizerMixLandModel
            {
                LandId = id,
                Date = DateTime.UtcNow,
                FertilizerMixId = mixId
            });

            await context.FertilizerMixLand.AddRangeAsync(models);

            await dbRepo.SaveChangesAsync();
            await dbRepo.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await dbRepo.RollbackTransactionAsync();
            throw;
        }
    }
    public async Task<List<LandDto>> GetMixLandsAsync(string landTitle, string mixTitle, DateTime? mixedDate)
    {
        var mixedLands = await landRepo.GetAllAsync(landTitle, mixTitle, mixedDate, true, null, false, true, true);

        var x1 = mixedLands.Where(l => string.IsNullOrEmpty(mixTitle)
            || l.FertilizerMixLands.Count == 0
            || l.Children.Any(ch => ch.FertilizerMixLands.Count == 0)).ToList();

        return x1;
    }
    public async Task RemoveMixLandAsync(long mixLandId) //ToDo: 
    {
        try
        {
            await dbRepo.BeginTransactionAsync();

            var mixLand = await context.FertilizerMixLand.Where(m => m.Id == mixLandId).Include(m => m.FertilizerMix)
                .ThenInclude(fm => fm.FertilizerMixDetails).FirstOrDefaultAsync();

            if (mixLand == null)
                throw new NotFoundException("Mix not found..");
            //// error 
            //Dictionary<long, double> dic = mixLand.FertilizerMix.FertilizerMixDetails.ToDictionary(x => x.FertilizerId, x => x.Quantity);

            //foreach (var item in dic)
            //    await fertilizerRepo.UpdateStoreAsync(item.Key, item.Value, DateTime.UtcNow, false);

            mixLand.IsValid = false;
            //await context.FertilizerMixLand.Where(fl => fl.Id == mixLandId && fl.IsValid).ExecuteUpdateAsync(fl => fl.SetProperty(fl => fl.IsValid, false));

            await dbRepo.SaveChangesAsync();
            await dbRepo.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await dbRepo.RollbackTransactionAsync();
            throw;
        }
    }
    public async Task RemoveMixLandsAsync(List<long> mixLandIds) //ToDo: 
     => await context.FertilizerMixLand.Where(fl => mixLandIds.Contains(fl.Id) && fl.IsValid).ExecuteUpdateAsync(fl => fl.SetProperty(fl => fl.IsValid, false));
    #endregion
}