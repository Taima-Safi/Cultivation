using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Color;
using Cultivation.Dto.Cutting;
using Cultivation.Dto.CuttingLand;
using Cultivation.Dto.Insecticide;
using Cultivation.Dto.InsecticideLand;
using Cultivation.Dto.Land;
using Cultivation.Repository.Base;
using Cultivation.Repository.Insecticide;
using Cultivation.Repository.Land;
using FourthPro.Dto.Common;
using FourthPro.Shared.Exception;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cultivation.Repository.InsecticideLand;

public class InsecticideLandRepo : IInsecticideLandRepo
{
    private readonly CultivationDbContext context;
    private readonly ILandRepo landRepo;
    private readonly IInsecticideRepo insecticideRepo;
    private readonly IBaseRepo<InsecticideLandModel> baseRepo;

    public InsecticideLandRepo(CultivationDbContext context, IBaseRepo<InsecticideLandModel> baseRepo, ILandRepo landRepo, IInsecticideRepo insecticideRepo)
    {
        this.context = context;
        this.baseRepo = baseRepo;
        this.landRepo = landRepo;
        this.insecticideRepo = insecticideRepo;
    }
    public async Task AddAsync(InsecticideLandFormDto dto)
    {
        if (!await landRepo.CheckIfExistByIdsAsync(dto.LandIds))
            throw new NotFoundException("One of lands not found..");

        if (!await insecticideRepo.CheckIfExistByIdsAsync(dto.Mixes.Select(x => x.InsecticideId).ToList()))
            throw new NotFoundException("One of insecticides not found..");

        //string fileName = null;
        //if (dto.File != null)
        //    fileName = FileHelper.FileHelper.UploadFile(dto.File, FileType.InsecticideLand);

        //var model = await context.InsecticideLand.AddAsync(new InsecticideLandModel
        //{
        //    Note = dto.Note,
        //    //File = fileName,
        //    Date = dto.Date,
        //    Liter = dto.Liter,
        //    LandId = dto.LandId,
        //    Quantity = dto.Quantity,
        //    InsecticideId = dto.InsecticideId,
        //});

        List<InsecticideLandModel> models = [];
        foreach (var landId in dto.LandIds)
        {
            models.AddRange(dto.Mixes.Select(i => new InsecticideLandModel
            {
                LandId = landId,
                Date = dto.Date,
                Liter = i.Liter,
                Note = dto.Note,
                Quantity = i.Quantity,
                InsecticideId = i.InsecticideId,
            }).ToList());
        }
        await context.InsecticideLand.AddRangeAsync(models);
        await context.SaveChangesAsync();
    }
    public async Task<CommonResponseDto<List<GroupedInsecticideLandDto>>> GetAllAsync(/*DateTime? date, */string note, double? liter, double? quantity, DateTime? from, DateTime? to
        , long? landId, long? insecticideId, int pageSize, int pageNum)
    {
        Expression<Func<InsecticideLandModel, bool>> expression = il => (!insecticideId.HasValue || il.InsecticideId == insecticideId) && (!landId.HasValue || il.LandId == landId)
        && (!quantity.HasValue || il.Quantity == quantity) && (!liter.HasValue || il.Liter == liter)/* && (!date.HasValue || il.Date == date)*/
        && (!from.HasValue || il.Date.Date >= from) && (!to.HasValue || il.Date.Date <= to) && (string.IsNullOrEmpty(note) || il.Note.Contains(note)) && il.IsValid;

        var x = await context.InsecticideLand.Where(expression)
            .Include(fl => fl.Land).Include(fl => fl.Insecticide)
            .OrderByDescending(fl => fl.LandId).ToListAsync();

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
                    Land = new LandDto
                    {
                        Id = il.Land.Id,
                        Size = il.Land.Size,
                        Title = il.Land.Title,
                        Location = il.Land.Location,
                        ParentId = il.Land.ParentId,
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


        var result = await context.InsecticideLand.Where(fl => (date.HasValue ? fl.Date == date : fl.Date == DateTime.UtcNow) && fl.IsValid)
            .Include(fl => fl.Land).Include(fl => fl.Insecticide).ToListAsync();

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
            //File = il.File,
            Land = new LandDto
            {
                Id = il.Land.Id,
                Size = il.Land.Size,
                Title = il.Land.Title,
                Location = il.Land.Location,
                ParentId = il.Land.ParentId,
                CuttingLands = il.Land.CuttingLands.Where(x => x.IsValid).Select(c => new CuttingLandDto
                {
                    CuttingColor = new CuttingColorDto
                    {
                        Cutting = new CuttingDto
                        {
                            Id = c.CuttingColor.Cutting.Id,
                            Type = c.CuttingColor.Cutting.Type,
                            Title = c.CuttingColor.Cutting.Title,
                        },
                        Color = new ColorDto
                        {
                            Id = c.CuttingColor.Color.Id,
                            Code = c.CuttingColor.Color.Code,
                            Title = c.CuttingColor.Color.Title,
                        }
                    }
                }).ToList(),
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
}
