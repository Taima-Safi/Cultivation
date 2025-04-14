using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Common;
using Cultivation.Dto.Fertilizer;
using Cultivation.Repository.Base;
using Cultivation.Repository.DataBase;
using Cultivation.Shared.Exception;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cultivation.Repository.Fertilizer;

public class FertilizerRepo : IFertilizerRepo
{
    private readonly IDbRepo dbRepo;
    private readonly IBaseRepo<FertilizerMixModel> mixBaseRepo;
    private readonly IBaseRepo<FertilizerModel> baseRepo;
    private readonly IBaseRepo<FertilizerStoreModel> storeBaseRepo;
    private readonly IBaseRepo<FertilizerTransactionModel> transactionBaseRepo;
    private readonly CultivationDbContext context;
    public FertilizerRepo(CultivationDbContext context, IBaseRepo<FertilizerModel> baseRepo, IDbRepo dbRepo, IBaseRepo<FertilizerTransactionModel> transactionBaseRepo, IBaseRepo<FertilizerMixModel> mixBaseRepo, IBaseRepo<FertilizerStoreModel> storeBaseRepo)
    {
        this.context = context;
        this.baseRepo = baseRepo;
        this.dbRepo = dbRepo;
        this.transactionBaseRepo = transactionBaseRepo;
        this.mixBaseRepo = mixBaseRepo;
        this.storeBaseRepo = storeBaseRepo;
    }

    public async Task<long> AddAsync(FertilizerFormDto dto)
    {
        //        string fileName = null;
        //        if (dto.File != null)
        //            fileName = FileHelper.FileHelper.UploadFile(dto.File, FileType.Fertilizer);

        var model = await context.Fertilizer.AddAsync(new FertilizerModel
        {
            NPK = dto.NPK,
            //File = fileName,
            Title = dto.Title,
            PublicTitle = dto.PublicTitle,
            Description = dto.Description
        });
        await context.SaveChangesAsync();
        return model.Entity.Id;
    }
    public async Task<CommonResponseDto<List<FertilizerDto>>> GetAllAsync(string npk, string title, string publicTitle,
        string description, int pageSize, int pageNum)
    {
        Expression<Func<FertilizerModel, bool>> expression = f => (string.IsNullOrEmpty(npk) || f.NPK.Contains(npk))
        && (string.IsNullOrEmpty(publicTitle) || f.PublicTitle.Contains(publicTitle))
        && (string.IsNullOrEmpty(description) || f.Description.Contains(description))
        && (string.IsNullOrEmpty(title) || f.Title.Contains(title))
        && f.IsValid;

        var x = await context.Fertilizer.Where(expression)
            .Skip(pageNum * pageSize)
            .Take(pageSize)
            .Select(f => new FertilizerDto
            {
                Id = f.Id,
                NPK = f.NPK,
                //File = f.File,
                Title = f.Title,
                PublicTitle = f.PublicTitle,
                Description = f.Description,
            }).ToListAsync();

        bool hasNextPage = false;
        if (x.Count > 0)
            hasNextPage = await baseRepo.CheckIfHasNextPageAsync(f => f.IsValid, pageSize, pageNum);

        return new CommonResponseDto<List<FertilizerDto>>(x, hasNextPage);
    }

    public async Task<FertilizerDto> GetByIdAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Fertilizer not found..");

        return await context.Fertilizer.Where(f => f.Id == id && f.IsValid).Select(f => new FertilizerDto
        {
            Id = f.Id,
            NPK = f.NPK,
            //File = f.File,
            Title = f.Title,
            PublicTitle = f.PublicTitle,
            Description = f.Description,
        }).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(long id, FertilizerFormDto dto)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Fertilizer not found..");

        await context.Fertilizer.Where(f => f.Id == id && f.IsValid).ExecuteUpdateAsync(f => f.SetProperty(f => f.NPK, dto.NPK)
        .SetProperty(f => f.Title, dto.Title).SetProperty(f => f.PublicTitle, dto.PublicTitle)
        .SetProperty(f => f.Description, dto.Description));
    }
    public async Task RemoveAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Fertilizer not found..");

        await context.Fertilizer.Where(f => f.Id == id && f.IsValid).ExecuteUpdateAsync(f => f.SetProperty(f => f.IsValid, false));
    }
    public async Task<bool> CheckIfExistAsync(long id)
    => await context.Fertilizer.Where(cl => cl.Id == id && cl.IsValid).AnyAsync();
    public async Task<bool> CheckIfExistByIdsAsync(List<long> ids)
        => await context.Fertilizer.AnyAsync(l => ids.Contains(l.Id) && l.IsValid);

    #region Store

    public async Task AddFertilizersToStore(Dictionary<long, double> addToStoreDic)
    {
        if (addToStoreDic.Count > 0)
        {
            var newFertilizers = addToStoreDic.Select(x => new FertilizerStoreModel
            {
                FertilizerId = x.Key,
                TotalQuantity = x.Value
            }).ToList();

            await context.FertilizerStore.AddRangeAsync(newFertilizers);
        }
    }
    private async Task AddFertilizerTransactionsAsync(Dictionary<long, double> dictionary, DateTime date, bool isAdd)
    {
        var transactions = dictionary.Select(x => new FertilizerTransactionModel
        {
            Date = date,
            IsAdd = isAdd,
            FertilizerId = x.Key,
            QuantityChange = x.Value
        }).ToList();

        await context.FertilizerTransaction.AddRangeAsync(transactions);
    }

    public async Task<CommonResponseDto<List<FertilizerStoreDto>>> GetAllFertilizerStoreAsync(string fertilizerTitle, string npk, DateTime? date, int pageSize, int pageNum)
    {
        Expression<Func<FertilizerStoreModel, bool>> expression = f => (string.IsNullOrEmpty(fertilizerTitle) || f.Fertilizer.Title.Contains(fertilizerTitle))
        && (string.IsNullOrEmpty(npk) || f.Fertilizer.NPK.Contains(npk))
        && (!date.HasValue || f.CreateDate <= date)
        && f.IsValid;

        var x = await context.FertilizerStore.Where(expression)
        .Skip(pageNum * pageSize)
        .Take(pageSize)
        .Select(f => new FertilizerStoreDto
        {
            Id = f.Id,
            //Date = f.Date,
            Fertilizer = new FertilizerDto
            {
                Id = f.Fertilizer.Id,
                NPK = f.Fertilizer.NPK,
                Title = f.Fertilizer.Title,
                Description = f.Fertilizer.Description,
                PublicTitle = f.Fertilizer.PublicTitle,
            }
        }).ToListAsync();

        bool hasNextPage = false;
        if (x.Count > 0)
            hasNextPage = await storeBaseRepo.CheckIfHasNextPageAsync(f => f.IsValid, pageSize, pageNum);

        return new CommonResponseDto<List<FertilizerStoreDto>>(x, hasNextPage);
    }
    public async Task UpdateStoreAsync(long fertilizerId, double quantity, DateTime date, bool isAdd)
    {
        try
        {
            await dbRepo.BeginTransactionAsync();

            if (quantity < 0)
                throw new NotFoundException("Quantity can't be less than or equal 0..");

            var fertilizerStoreModel = await context.FertilizerStore.Where(x => x.FertilizerId == fertilizerId && x.IsValid).FirstOrDefaultAsync();
            if (fertilizerStoreModel == null)
            {
                if (!isAdd)
                    throw new NotFoundException("Fertilizer not found in depot, you can't pull..");

                await AddFertilizersToStore(new Dictionary<long, double> { { fertilizerId, quantity } });
            }
            else
            {
                if (!isAdd && fertilizerStoreModel.TotalQuantity < quantity)
                    throw new NotFoundException("Fertilizer quantity is less than you want..");

                fertilizerStoreModel.TotalQuantity += isAdd ? quantity : -quantity;
            }

            await AddFertilizerTransactionsAsync(new Dictionary<long, double> { { fertilizerId, quantity } }, date, isAdd);


            await context.SaveChangesAsync();
            await dbRepo.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await dbRepo.RollbackTransactionAsync();
            throw;
        }
    }
    public async Task UpdateStoreAsync2(Dictionary<long, double> fertilizersDic, DateTime date, bool isAdd)
    {
        try
        {
            await dbRepo.BeginTransactionAsync();

            if (fertilizersDic.Any(x => x.Value < 0))
                throw new NotFoundException("One of quantity is less than 0..");

            var fertilizerStoreModels = await context.FertilizerStore.Where(x => fertilizersDic.Keys.Contains(x.FertilizerId) && x.IsValid).ToListAsync();
            // For pulling (!isAdd), ensure all requested fertilizers exist in store
            if (!isAdd && fertilizerStoreModels.Count != fertilizersDic.Count)
            {
                var missingIds = fertilizersDic.Keys.Except(fertilizerStoreModels.Select(f => f.FertilizerId));
                throw new NotFoundException($"Fertilizers not found in store: {string.Join(", ", missingIds)}");
            }

            // Check if pulling more than available
            if (!isAdd)
            {
                var insufficient = fertilizerStoreModels
                    .Where(f => f.TotalQuantity < fertilizersDic[f.FertilizerId])
                    .Select(f => f.FertilizerId);

                if (insufficient.Any())
                    throw new NotFoundException($"Insufficient quantity for fertilizers: {string.Join(", ", insufficient)}");
            }
            ////check if all fertilizer in store have required quantity
            //if (!isAdd && fertilizersDic.Any(dic => fertilizerStoreModels.Any(f => f.FertilizerId == dic.Key && f.TotalQuantity < dic.Value)))
            //    throw new NotFoundException("One of Fertilizer quantity is less than you want..");

            //Dictionary<long, double> toAddDic = new Dictionary<long, double>();
            //Dictionary<long, double> toUpdateDic = new Dictionary<long, double>();

            // Separate into ADD (new fertilizers) and UPDATE (existing fertilizers)
            var toAddDic = fertilizersDic
                .Where(x => isAdd && !fertilizerStoreModels.Any(f => f.FertilizerId == x.Key))
                .ToDictionary(x => x.Key, x => x.Value);

            var toUpdateDic = fertilizersDic
                .Where(x => fertilizerStoreModels.Any(f => f.FertilizerId == x.Key))
                .ToDictionary(x => x.Key, x => x.Value);


            //foreach (var item in fertilizersDic)
            //    if (isAdd && !fertilizerStoreModels.Any(f => f.FertilizerId == item.Key))
            //        toAddDic.Add(item.Key, item.Value);
            //    else
            //        toUpdateDic.Add(item.Key, item.Value);

            if (toAddDic.Count > 0)
                await AddFertilizersToStore(toAddDic);

            foreach (var storeItem in fertilizerStoreModels)
            {
                if (toUpdateDic.TryGetValue(storeItem.FertilizerId, out double quantity))
                {
                    storeItem.TotalQuantity += isAdd ? quantity : -quantity;

                    // Prevent negative stock
                    if (storeItem.TotalQuantity < 0)
                        throw new InvalidOperationException($"Negative stock not allowed for fertilizer {storeItem.FertilizerId}");
                }
            }

            await AddFertilizerTransactionsAsync(fertilizersDic, date, isAdd);

            await context.SaveChangesAsync();
            await dbRepo.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await dbRepo.RollbackTransactionAsync();
            throw;
        }
    }
    public async Task UpdateStoreForMixAsync(long mixId, double donumNum, DateTime date)
    {
        try
        {
            await dbRepo.BeginTransactionAsync();

            var mixModels = await context.FertilizerMixDetail.Where(m => m.FertilizerMixId == mixId && m.IsValid).ToListAsync() ??
                throw new NotFoundException("mix not found..");

            //foreach (var model in mixModels)
            //{
            //    await UpdateStoreAsync(model.FertilizerId, model.Quantity * donumNum, date, false);
            //}
            var toUpdateStoreDic = mixModels.ToDictionary(x => x.FertilizerId, x => x.Quantity * donumNum);
            await UpdateStoreAsync2(toUpdateStoreDic, date, false);
        }
        catch (Exception)
        {
            await dbRepo.RollbackTransactionAsync();
            throw;
        }
    }
    public async Task<CommonResponseDto<List<FertilizerTransactionDto>>> GetAllFertilizerTransactionAsync(string fertilizerTitle, DateTime? from, DateTime? to,
        int pageSize, int pageNum)
    {
        Expression<Func<FertilizerTransactionModel, bool>> expression = f => (string.IsNullOrEmpty(fertilizerTitle) || f.Fertilizer.Title.Contains(fertilizerTitle))
        && (!from.HasValue || f.Date >= from)
        && (!to.HasValue || f.Date <= to)
        && f.IsValid;

        var x = await context.FertilizerTransaction.Where(expression)
            .Skip(pageNum * pageSize)
            .Take(pageSize)
            .Select(f => new FertilizerTransactionDto
            {
                Id = f.Id,
                Date = f.Date,
                IsAdd = f.IsAdd,
                QuantityChange = f.QuantityChange,
                Fertilizer = new FertilizerDto
                {
                    Id = f.Fertilizer.Id,
                    NPK = f.Fertilizer.NPK,
                    Title = f.Fertilizer.Title,
                    Description = f.Fertilizer.Description,
                    PublicTitle = f.Fertilizer.PublicTitle,
                }
            }).ToListAsync();

        bool hasNextPage = false;
        if (x.Count > 0)
            hasNextPage = await transactionBaseRepo.CheckIfHasNextPageAsync(f => f.IsValid, pageSize, pageNum);

        return new CommonResponseDto<List<FertilizerTransactionDto>>(x, hasNextPage);
    }
    #endregion
}
