using Cultivation.Database.Context;
using Cultivation.Database.Model;
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
using FourthPro.Dto.Common;
using FourthPro.Shared.Exception;
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

    public InsecticideLandRepo(CultivationDbContext context, IBaseRepo<InsecticideLandModel> baseRepo, ILandRepo landRepo, IInsecticideRepo insecticideRepo,
        ICuttingLandRepo cuttingLandRepo, IFileRepo<InsecticideExportToExcelDto> fileRepo)
    {
        this.context = context;
        this.baseRepo = baseRepo;
        this.landRepo = landRepo;
        this.fileRepo = fileRepo;
        this.insecticideRepo = insecticideRepo;
        this.cuttingLandRepo = cuttingLandRepo;
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
            il.CuttingLand.LandId == landId && il.IsValid &&
            (
                (!from.HasValue && !to.HasValue && il.CuttingLand.IsActive) || // If both are null, check IsActive
                (from.HasValue && il.Date.Date >= from) ||                     // If from has a value, check Date >= from
                (to.HasValue && il.Date.Date <= to)                            // If to has a value, check Date <= to
            );
        var result = await context.InsecticideLand.Where(expression).Include(il => il.Insecticide).ToListAsync();

        return new CommonResponseDto<List<InsecticideLandModel>>(result);
    }

    public async Task AddAsync(InsecticideLandFormDto dto)
    {

        if (!await cuttingLandRepo.CheckIfExistByIdsAsync(dto.CuttingLandIds))
            throw new NotFoundException("One of Cutting Lands not found..");

        if (!await insecticideRepo.CheckIfExistByIdsAsync(dto.Mixes.Select(x => x.InsecticideId).ToList()))
            throw new NotFoundException("One of insecticides not found..");

        List<InsecticideLandModel> models = [];
        foreach (var cuttingLandId in dto.CuttingLandIds)
        {
            models.AddRange(dto.Mixes.Select(i => new InsecticideLandModel
            {
                Date = dto.Date,
                Liter = i.Liter,
                Note = dto.Note,
                Quantity = i.Quantity,
                CuttingLandId = cuttingLandId,
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

        && (!landId.HasValue || il.CuttingLand.LandId == landId) && il.IsValid &&
            (
                (!from.HasValue && !to.HasValue && il.CuttingLand.IsActive) || // If both are null, check IsActive
                (from.HasValue && il.Date.Date >= from) ||                     // If from has a value, check Date >= from
                (to.HasValue && il.Date.Date <= to)                            // If to has a value, check Date <= to
            );
        /*  && (!from.HasValue || il.Date.Date >= from) && (!to.HasValue || il.Date.Date <= to)*/

        var x = await context.InsecticideLand.Where(expression)
            .Include(il => il.CuttingLand).ThenInclude(il => il.Land).Include(il => il.CuttingLand).ThenInclude(il => il.CuttingColor)
            .Include(il => il.Insecticide)
            .OrderByDescending(il => il.CuttingLand.LandId).ToListAsync();

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
                    //File = il.File,
                    CuttingLand = new CuttingLandDto
                    {
                        Id = il.CuttingLand.Id,
                        Date = il.CuttingLand.Date,
                        Quantity = il.CuttingLand.Quantity,
                        Land = new LandDto
                        {
                            Id = il.CuttingLand.Land.Id,
                            Size = il.CuttingLand.Land.Size,
                            Title = il.CuttingLand.Land.Title,
                            Location = il.CuttingLand.Land.Location,
                            ParentId = il.CuttingLand.Land.ParentId,
                        },
                        CuttingColor = new CuttingColorDto
                        {
                            Id = il.CuttingLand.CuttingColor.Id,
                            Code = il.CuttingLand.CuttingColor.Code,
                        }
                    },
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
            .Include(il => il.CuttingLand).ThenInclude(il => il.Land).Include(il => il.Insecticide).ToListAsync();

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
            //File = il.File,
            CuttingLand = new CuttingLandDto
            {
                Id = il.CuttingLand.Id,
                Date = il.CuttingLand.Date,
                Quantity = il.CuttingLand.Quantity,
                Land = new LandDto
                {
                    Id = il.CuttingLand.Land.Id,
                    Size = il.CuttingLand.Land.Size,
                    Title = il.CuttingLand.Land.Title,
                    Location = il.CuttingLand.Land.Location,
                    ParentId = il.CuttingLand.Land.ParentId,
                },
                CuttingColor = new CuttingColorDto
                {
                    Id = il.CuttingLand.CuttingColor.Id,
                    Code = il.CuttingLand.CuttingColor.Code,
                }
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

        if (!await cuttingLandRepo.CheckIfExistAsync(dto.CuttingLandId))
            throw new NotFoundException("cutting not found..");

        if (!await insecticideRepo.CheckIfExistAsync(dto.InsecticideId))
            throw new NotFoundException("Fertilizer not has this fertilizer..");

        await context.InsecticideLand.Where(il => il.Id == id && il.IsValid).ExecuteUpdateAsync(il => il.SetProperty(il => il.Note, dto.Note)
        .SetProperty(il => il.Quantity, dto.Quantity).SetProperty(il => il.Liter, dto.Liter).SetProperty(il => il.Date, dto.Date)
        .SetProperty(il => il.InsecticideId, dto.InsecticideId).SetProperty(il => il.CuttingLandId, dto.CuttingLandId));
    }
    public async Task RemoveAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Land not has this insecticide...");

        await context.InsecticideLand.Where(il => il.Id == id && il.IsValid).ExecuteUpdateAsync(il => il.SetProperty(il => il.IsValid, false));
    }

    public async Task<bool> CheckIfExistAsync(long id)
        => await context.InsecticideLand.AnyAsync(il => il.Id == id && il.IsValid);
}
