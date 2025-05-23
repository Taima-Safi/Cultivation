﻿using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Common;
using Cultivation.Dto.Cutting;
using Cultivation.Dto.CuttingLand;
using Cultivation.Dto.Insecticide;
using Cultivation.Dto.InsecticideLand;
using Cultivation.Dto.Land;
using Cultivation.Repository.Base;
using Cultivation.Repository.CuttingLand;
using Cultivation.Repository.File;
using Cultivation.Repository.Insecticide;
using Cultivation.Repository.Land;
using Cultivation.Shared.Enum;
using Cultivation.Shared.Exception;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cultivation.Repository.InsecticideLand;

public class InsecticideLandRepo : IInsecticideLandRepo
{
    private readonly CultivationDbContext context;
    private readonly ICuttingLandRepo cuttingLandRepo;
    private readonly ILandRepo landRepo;
    private readonly IFileRepo<InsecticideExportToExcelDto> fileRepo;
    private readonly IInsecticideRepo insecticideRepo;
    private readonly IBaseRepo<InsecticideLandModel> baseRepo;
    private readonly IBaseRepo<InsecticideMixModel> mixBaseRepo;
    private readonly IBaseRepo<CuttingLandModel> cuttingLandBaseRepo;
    private readonly IBaseRepo<LandModel> landBaseRepo;


    public InsecticideLandRepo(CultivationDbContext context, IBaseRepo<InsecticideLandModel> baseRepo, ILandRepo landRepo, IInsecticideRepo insecticideRepo,
        ICuttingLandRepo cuttingLandRepo, IFileRepo<InsecticideExportToExcelDto> fileRepo, IBaseRepo<InsecticideMixModel> mixBaseRepo, IBaseRepo<CuttingLandModel> cuttingLandBaseRepo, IBaseRepo<LandModel> landBaseRepo)
    {
        this.context = context;
        this.baseRepo = baseRepo;
        this.landRepo = landRepo;
        this.fileRepo = fileRepo;
        this.insecticideRepo = insecticideRepo;
        this.cuttingLandRepo = cuttingLandRepo;
        this.mixBaseRepo = mixBaseRepo;
        this.cuttingLandBaseRepo = cuttingLandBaseRepo;
        this.landBaseRepo = landBaseRepo;
    }
    public async Task<(FormFile file, MemoryStream stream)> ExportExcelAsync(long landId, DateTime? from, DateTime? to, string fileName)
    {
        var result = await GetInsecticidesLandModelAsync(landId, from, to);
        var toExport = result.Data.Select(x => new InsecticideExportToExcelDto
        {
            Type = x.Insecticide.Type.ToString(),
            Date = x.Date.ToShortDateString(),
            Quantity = x.Quantity.ToString(),
            Liter = x.Liter.ToString(),
            Title = x.Insecticide.Title,
            Description = x.Insecticide.Description,
            PublicTitle = x.Insecticide.PublicTitle,
        }).ToList();

        var excel = fileRepo.ExportToExcel(ExportType.LandFertilizers, fileName, new(), /*filtersPropertiesNames,*/ toExport);
        return excel;
    }
    public async Task<CommonResponseDto<List<InsecticideLandModel>>> GetInsecticidesLandModelAsync(long landId, DateTime? from, DateTime? to)
    {
        if (!await landRepo.CheckIfExistAsync(landId))
            throw new NotFoundException("Land not found..");

        Expression<Func<InsecticideLandModel, bool>> expression = il =>
            il.LandId == landId && il.IsValid &&
            (
                (!from.HasValue && !to.HasValue && il.Land.CuttingLands.Any(c => c.IsActive)) || // If both are null, check IsActive
                (from.HasValue && il.Date.Date >= from) ||                     // If from has a value, check Date >= from
                (to.HasValue && il.Date.Date <= to)                            // If to has a value, check Date <= to
            );
        var result = await context.InsecticideLand.Where(expression).Include(il => il.Insecticide).ToListAsync();

        return new CommonResponseDto<List<InsecticideLandModel>>(result);
    }

    public async Task AddAsync(InsecticideLandFormDto dto)
    {

        if (!await landRepo.CheckIfExistByIdsAsync(dto.LandIds))
            throw new NotFoundException("One of Lands not found..");

        if (!await insecticideRepo.CheckIfExistByIdsAsync(dto.Mixes.Select(x => x.InsecticideId).ToList()))
            throw new NotFoundException("One of insecticides not found..");

        List<InsecticideLandModel> models = [];
        foreach (var landId in dto.LandIds)
        {
            models.AddRange(dto.Mixes.Select(i => new InsecticideLandModel
            {
                Date = dto.Date,
                Liter = i.Liter,
                Note = dto.Note,
                LandId = landId,
                Quantity = i.Quantity,
                InsecticideId = i.InsecticideId,
            }).ToList());
        }
        await context.InsecticideLand.AddRangeAsync(models);
        await context.SaveChangesAsync();
    }
    public async Task<CommonResponseDto<List<GroupedInsecticideLandDto>>> GetAllAsync(string note, double? liter, double? quantity, DateTime? from, DateTime? to
        , long? landId, long? insecticideId, int pageSize, int pageNum)
    {
        Expression<Func<InsecticideLandModel, bool>> expression = il => (!insecticideId.HasValue || il.InsecticideId == insecticideId)
        && (!quantity.HasValue || il.Quantity == quantity) && (!liter.HasValue || il.Liter == liter)
        && (string.IsNullOrEmpty(note) || il.Note.Contains(note)) && il.IsValid

        && (!landId.HasValue || il.LandId == landId) && il.IsValid &&
            (
                (!from.HasValue && !to.HasValue && il.Land.CuttingLands.Any(c => c.IsActive)) || // If both are null, check IsActive
                (from.HasValue && il.Date.Date >= from) ||                     // If from has a value, check Date >= from
                (to.HasValue && il.Date.Date <= to)                            // If to has a value, check Date <= to
            );
        /*  && (!from.HasValue || il.Date.Date >= from) && (!to.HasValue || il.Date.Date <= to)*/

        var x = await context.InsecticideLand.Where(expression)
            .Include(il => il.Insecticide).Include(il => il.Land).ThenInclude(il => il.CuttingLands).ThenInclude(il => il.CuttingColor)
            .OrderByDescending(il => il.LandId).ToListAsync();

        var result = x
            .GroupBy(group => group.Date)
            .OrderByDescending(fl => fl.Key)
            .Skip(pageSize * pageNum)
            .Take(pageSize)
            .Select(il => new GroupedInsecticideLandDto
            {
                Date = il.Key,
                InsecticideLand = il.Select(il => new InsecticideLandDto
                {
                    Id = il.Id,
                    //Date = il.Date,
                    Note = il.Note,
                    Liter = il.Liter,
                    Quantity = il.Quantity,
                    Land = new LandDto
                    {
                        Id = il.Land.Id,
                        Size = il.Land.Size,
                        Title = il.Land.Title,
                        ParentId = il.Land.ParentId,
                        Location = il.Land.Location,
                        CuttingLands = il.Land.CuttingLands.Where(cl => cl.IsValid).Select(cl => new CuttingLandDto
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
                    //    Id = il.CuttingLand.Id,
                    //    Date = il.CuttingLand.Date,
                    //    Quantity = il.CuttingLand.Quantity,
                    //    Land = new LandDto
                    //    {
                    //        Id = il.CuttingLand.Land.Id,
                    //        Size = il.CuttingLand.Land.Size,
                    //        Title = il.CuttingLand.Land.Title,
                    //        Location = il.CuttingLand.Land.Location,
                    //        ParentId = il.CuttingLand.Land.ParentId,
                    //    },
                    //    CuttingColor = new CuttingColorDto
                    //    {
                    //        Id = il.CuttingLand.CuttingColor.Id,
                    //        Code = il.CuttingLand.CuttingColor.Code,
                    //    }
                    //},
                    Insecticide = new InsecticideDto
                    {
                        Id = il.Insecticide.Id,
                        Type = il.Insecticide.Type,
                        Title = il.Insecticide.Title,
                        Description = il.Insecticide.Description,
                        PublicTitle = il.Insecticide.PublicTitle,
                    }
                }).ToList()
            }).ToList();

        var hasNextPage = false;
        if (result.Any())
            hasNextPage = await baseRepo.CheckIfHasNextPageAsync(expression, pageSize, pageNum);

        return new(result, hasNextPage);
    }
    public async Task<List<LandDto>> GetLandsWhichNotUsedInDayAsync(DateTime? date)
    {
        var landModels = await context.Land.Where(l => !l.Children.Any() && l.IsValid).ToListAsync();

        var result = await context.InsecticideLand.Where(il => (date.HasValue ? il.Date.Date == date : il.Date.Date == DateTime.UtcNow.Date) && il.IsValid)
            .Include(il => il.Land).Include(il => il.Insecticide).ToListAsync();

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
    public async Task<InsecticideLandDto> GetByIdAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Land not has this insecticide...");

        return await context.InsecticideLand.Where(il => il.Id == id && il.IsValid).Select(il => new InsecticideLandDto
        {
            Id = il.Id,
            Date = il.Date,
            Note = il.Note,
            Liter = il.Liter,
            Quantity = il.Quantity,
            Land = new LandDto
            {
                Id = il.Land.Id,
                Size = il.Land.Size,
                Title = il.Land.Title,
                ParentId = il.Land.ParentId,
                Location = il.Land.Location,
                CuttingLands = il.Land.CuttingLands.Where(cl => cl.IsValid).Select(cl => new CuttingLandDto
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
            Insecticide = new InsecticideDto
            {
                Id = il.Insecticide.Id,
                Type = il.Insecticide.Type,
                Title = il.Insecticide.Title,
                Description = il.Insecticide.Description,
                PublicTitle = il.Insecticide.PublicTitle,
            }
        }).FirstOrDefaultAsync();
    }
    public async Task UpdateAsync(long id, UpdateInsecticideLandFormDto dto)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Land not has this insecticide...");

        if (!await landRepo.CheckIfExistAsync(dto.LandId))
            throw new NotFoundException("land not found..");

        if (!await insecticideRepo.CheckIfExistAsync(dto.InsecticideId))
            throw new NotFoundException("Fertilizer not has this fertilizer..");

        await context.InsecticideLand.Where(il => il.Id == id && il.IsValid).ExecuteUpdateAsync(il => il.SetProperty(il => il.Note, dto.Note)
        .SetProperty(il => il.Quantity, dto.Quantity).SetProperty(il => il.Liter, dto.Liter).SetProperty(il => il.Date, dto.Date)
        .SetProperty(il => il.InsecticideId, dto.InsecticideId).SetProperty(il => il.LandId, dto.LandId));
    }
    public async Task RemoveAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Land not has this insecticide...");

        await context.InsecticideLand.Where(il => il.Id == id && il.IsValid).ExecuteUpdateAsync(il => il.SetProperty(il => il.IsValid, false));
    }

    public async Task<bool> CheckIfExistAsync(long id)
        => await context.InsecticideLand.AnyAsync(il => il.Id == id && il.IsValid);

    #region MixLand

    public async Task AddMixLandAsync(long mixId, long landId)
    {
        if (!await mixBaseRepo.CheckIfExistAsync(m => m.Id == mixId && m.IsValid))
            throw new NotFoundException("mix not found..");

        if (!await landBaseRepo.CheckIfExistAsync(m => m.Id == landId && m.IsValid))
            throw new NotFoundException("land not found..");

        await context.InsecticideMixLand.AddAsync(new InsecticideMixLandModel
        {
            LandId = landId,
            Date = DateTime.UtcNow,
            InsecticideMixId = mixId,
        });
        await context.SaveChangesAsync();
    }
    public async Task AddMixLandsAsync(long mixId, List<long> landIds)
    {
        if (!await mixBaseRepo.CheckIfExistAsync(m => m.Id == mixId && m.IsValid))
            throw new NotFoundException("mix not found..");

        if (!await landBaseRepo.CheckIfExistAsync(m => landIds.Contains(m.Id) && m.IsValid))
            throw new NotFoundException("one of lands not found..");
        List<InsecticideMixLandModel> models = [];
        foreach (var id in landIds)
            models.Add(new InsecticideMixLandModel
            {
                LandId = id,
                Date = DateTime.UtcNow,
                InsecticideMixId = mixId
            });
        await context.InsecticideMixLand.AddRangeAsync(models);
        await context.SaveChangesAsync();
    }
    public async Task<List<LandDto>> GetMixLandsAsync(string landTitle, string mixTitle, DateTime? mixedDate)
    {
        var mixedLands = await landRepo.GetAllAsync(landTitle, mixTitle, mixedDate, false, null, false, true, true);

        //ToDo: fix filter..
        //var x = mixedLands.Where(l => string.IsNullOrEmpty(mixTitle) || l.Children.Any(ch => ch.CuttingLands.Any(cl => cl.InsecticideMixLands
        //                                                .Any(fml => fml.InsecticideMix.Title.Contains(mixTitle)
        //                                                       && (!mixedDate.HasValue || fml.Date.Date == mixedDate?.Date))))).ToList();
        var x1 = mixedLands.Where(l =>
            string.IsNullOrEmpty(mixTitle)
            || l.InsecticideMixLands.Count == 0
            || l.Children.Any(ch => ch.InsecticideMixLands.Count == 0)).ToList();

        return x1;
    }
    public async Task RemoveMixLandAsync(long mixLandId) //ToDo: 
     => await context.InsecticideMixLand.Where(fl => fl.Id == mixLandId && fl.IsValid).ExecuteUpdateAsync(fl => fl.SetProperty(fl => fl.IsValid, false));
    #endregion
}
