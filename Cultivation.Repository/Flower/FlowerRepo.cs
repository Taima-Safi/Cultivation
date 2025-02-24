using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Color;
using Cultivation.Dto.Common;
using Cultivation.Dto.Cutting;
using Cultivation.Dto.CuttingLand;
using Cultivation.Dto.Flower;
using Cultivation.Dto.Land;
using Cultivation.Repository.Base;
using Cultivation.Repository.DataBase;
using Cultivation.Shared.Exception;
using Microsoft.EntityFrameworkCore;
using FlowerModel = Cultivation.Database.Model.FlowerModel;

namespace Cultivation.Repository.Flower;

public class FlowerRepo : IFlowerRepo
{
    private readonly CultivationDbContext context;
    private readonly IBaseRepo<FertilizerLandModel> baseRepo;
    private readonly IDbRepo dbRepo;

    public FlowerRepo(CultivationDbContext context, IBaseRepo<FertilizerLandModel> baseRepo, IDbRepo dbRepo)
    {
        this.context = context;
        this.baseRepo = baseRepo;
        this.dbRepo = dbRepo;
    }
    public async Task AddAsync(AddFlowerFormDto dto, long cuttingLandId)
    {
        await dbRepo.BeginTransactionAsync();
        try
        {
            //ToDo: send from front cuttingColor code with landId
            var cuttingCode = await context.CuttingLand.Where(c => c.Id == cuttingLandId && c.IsValid)
                .Select(c => c.CuttingColor.Code).FirstOrDefaultAsync()
                ?? throw new NotFoundException("Cuttings not found.");

            var model = dto.Flowers.Select(f => new FlowerModel
            {
                Note = f.Note,
                Long = f.Long,
                Count = f.Count,
                Date = dto.Date,
                Worker = dto.Worker,
                CuttingLandId = cuttingLandId
            }).ToList();
            await context.Flower.AddRangeAsync(model);

            var store = await context.FlowerStore.Where(s => s.IsValid && s.Code == cuttingCode).ToDictionaryAsync(s => s.FlowerLong);
            foreach (var item in dto.Flowers)
            {
                //var storeLong = store.FirstOrDefault(s => s.FlowerLong == item.Long);
                if (!store.TryGetValue(item.Long, out var existingStore))
                {
                    await context.FlowerStore.AddAsync(new FlowerStoreModel
                    {
                        Code = cuttingCode,
                        Count = item.Count,
                        FlowerLong = item.Long,
                        TotalCount = item.Count,
                        RemainedCount = item.Count,
                    });
                }
                else
                {
                    existingStore.Count = existingStore.Count + item.Count;
                    existingStore.TotalCount = existingStore.TotalCount + item.Count;
                    existingStore.RemainedCount = existingStore.RemainedCount + item.Count;

                    // await context.FlowerStore.Where(s =>  s.Code == cuttingCode && s.FlowerLong == item.Long && s.IsValid).ExecuteUpdateAsync(s =>
                    // s.SetProperty(s => s.Count, newCount).SetProperty(s => s.RemainedCount, newRemainedCount));
                }
            }

            await dbRepo.SaveChangesAsync();
            await dbRepo.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await dbRepo.RollbackTransactionAsync();
            throw;
        }
    }
    public async Task<CommonResponseDto<List<FlowerDto>>> GetAllAsync(DateTime? from, DateTime? to, long? cuttingLandId, string cuttingTitle
        , string cuttingColorCode, string worker, double? Long, int pageSize, int pageNum)
    {
        var x = await context.Flower.Where(f =>
         (!from.HasValue || f.Date >= from)
        && (!to.HasValue || f.Date <= to)
        //&&
        // ((!from.HasValue && !to.HasValue) && f.CuttingLand.IsActive)
        && (!Long.HasValue || f.Long == Long)
        && (!cuttingLandId.HasValue || f.CuttingLandId == cuttingLandId)
        && (string.IsNullOrEmpty(worker) || f.Worker == worker)
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
                Long = c.Long,
                Count = c.Count,
                Worker = c.Worker,
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
            Long = c.Long,
            Count = c.Count,
            Worker = c.Worker,
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
    public async Task<double> GetFlowerAverageInDonumAsync()
    {
        var totalSize = await context.CuttingLand.Where(c => c.IsActive && c.IsValid).SumAsync(c => c.Land.Size); // get sum of children size for lands have cutting
        var totalFlowerCount = await context.Flower.Where(f => f.CuttingLand.IsActive && f.CuttingLand.IsValid).SumAsync(f => f.Count);

        if (totalFlowerCount <= 0)
            throw new NotFoundException("no flowers..");

        return totalFlowerCount / totalSize;
    }

    public async Task<List<FlowerModel>> GetModelsByIdsAsync(List<long> ids)
        => await context.Flower.Where(f => ids.Contains(f.Id) && f.IsValid).ToListAsync();
    public async Task<FlowerModel> GetModelByIdAsync(long id)
        => await context.Flower.Where(f => f.Id == id && f.IsValid).FirstOrDefaultAsync();
    public async Task UpdateAsync(long id, string note, string worker, double Long, int count, DateTime date)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Flowers not found..");

        await context.Flower.Where(c => c.Id == id && c.IsValid).ExecuteUpdateAsync(c => c.SetProperty(c => c.Date, date)
        .SetProperty(c => c.Note, note).SetProperty(c => c.Worker, worker).SetProperty(c => c.Long, Long).SetProperty(c => c.Count, count));
    }

    public async Task RemoveAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Flowers not found..");

        await context.Flower.Where(c => c.Id == id && c.IsValid).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsValid, false));
    }

    public async Task<bool> CheckIfExistAsync(long id)
     => await context.Flower.Where(c => c.Id == id && c.IsValid).AnyAsync();

    #region FlowerStore
    public async Task<CommonResponseDto<List<FlowerStoreDto>>> GetAllFlowerStoreAsync(string code, int pageSize, int pageNum)
    {
        var x = await context.FlowerStore.Where(c => (string.IsNullOrEmpty(code) || c.Code.Contains(code)) && c.IsValid)
        .Skip(pageNum * pageSize)
        .Take(pageSize)
        .Select(c => new FlowerStoreDto
        {
            Id = c.Id,
            Code = c.Code,
            Count = c.Count,
            FlowerLong = c.FlowerLong,
            TotalCount = c.TotalCount,
            TrashedCount = c.TrashedCount,
            RemainedCount = c.RemainedCount,
            ExternalCount = c.ExternalCount
        }).ToListAsync();

        bool hasNextPage = false;
        if (x.Count > 0)
            hasNextPage = await baseRepo.CheckIfHasNextPageAsync(fl => fl.IsValid, pageSize, pageNum);

        return new CommonResponseDto<List<FlowerStoreDto>>(x, hasNextPage);
    }
    public async Task<List<FlowerStoreModel>> GetFlowerStoreModelsByCodesAsync(List<string> codes)
        => await context.FlowerStore.Where(fs => codes.Contains(fs.Code) && fs.IsValid).ToListAsync();


    public async Task<List<FlowerStoreModel>> GetStoreModelsByIdsAsync(List<long> ids)
        => await context.FlowerStore.Where(f => ids.Contains(f.Id) && f.IsValid).ToListAsync();

    public async Task AddExternalFlowerAsync(long flowerStoreId, int count)
    {
        var model = await context.FlowerStore.Where(c => c.Id == flowerStoreId && c.IsValid).FirstOrDefaultAsync();

        model.Count += count;
        model.RemainedCount += count;
        model.ExternalCount += count;

        await context.SaveChangesAsync();
    }

    public async Task AddTrashedFlowerAsync(long flowerStoreId, int trashedCount)
    {
        var storeModel = await context.FlowerStore.FirstOrDefaultAsync(fs => fs.Id == flowerStoreId && fs.IsValid);
        if (storeModel == null)
            throw new NotFoundException("This code not found in store");

        if (storeModel.RemainedCount < trashedCount)
            throw new NotFoundException("This code of flower count not found in store");

        storeModel.Count -= trashedCount;
        storeModel.RemainedCount -= trashedCount;
        storeModel.TrashedCount += trashedCount;
        await context.SaveChangesAsync();
    }

    #endregion
}
