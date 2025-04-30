using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Common;
using Cultivation.Dto.Insecticide;
using Cultivation.Repository.Base;
using Cultivation.Repository.DataBase;
using Cultivation.Shared.Enum;
using Cultivation.Shared.Exception;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cultivation.Repository.Insecticide;

public class InsecticideRepo : IInsecticideRepo
{
    private readonly IDbRepo dbRepo;
    private readonly CultivationDbContext context;
    private readonly IBaseRepo<InsecticideModel> baseRepo;
    private readonly IBaseRepo<InsecticideStoreModel> storeBaseRepo;
    private readonly IBaseRepo<InsecticideTransactionModel> transactionBaseRepo;
    public InsecticideRepo(CultivationDbContext context, IBaseRepo<InsecticideModel> baseRepo, IBaseRepo<InsecticideStoreModel> storeBaseRepo, IBaseRepo<InsecticideTransactionModel> transactionBaseRepo, IDbRepo dbRepo)
    {
        this.context = context;
        this.baseRepo = baseRepo;
        this.storeBaseRepo = storeBaseRepo;
        this.transactionBaseRepo = transactionBaseRepo;
        this.dbRepo = dbRepo;
    }
    public async Task<long> AddAsync(InsecticideFormDto dto)
    {
        //string fileName = null;
        //if (dto.File != null)
        //    fileName = FileHelper.FileHelper.UploadFile(dto.File, FileType.Insecticide);

        var model = await context.Insecticide.AddAsync(new InsecticideModel
        {
            Type = dto.Type,
            // File = fileName,
            Title = dto.Title,
            PublicTitle = dto.PublicTitle,
            Description = dto.Description
        });
        await context.SaveChangesAsync();

        return model.Entity.Id;
    }
    public async Task UpdateAsync(long id, InsecticideFormDto dto)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Insecticide not found..");

        await context.Insecticide.Where(i => i.Id == id && i.IsValid).ExecuteUpdateAsync(i => i.SetProperty(i => i.Description, dto.Description)
        .SetProperty(i => i.Title, dto.Title).SetProperty(i => i.Type, dto.Type).SetProperty(i => i.PublicTitle, dto.PublicTitle));
    }
    public async Task<CommonResponseDto<List<InsecticideDto>>> GetAllAsync(string title, string publicTitle, string description,
        InsecticideType? type, int pageSize, int pageNum)
    {
        Expression<Func<InsecticideModel, bool>> expression = i => (string.IsNullOrEmpty(title) || i.Title.Contains(title))
        && (string.IsNullOrEmpty(publicTitle) || i.PublicTitle.Contains(publicTitle))
        && (string.IsNullOrEmpty(description) || i.Description.Contains(description))
        && (!type.HasValue || i.Type == type)
        && i.IsValid;

        var result = await context.Insecticide.Where(expression)
            .Skip(pageNum * pageSize)
            .Take(pageSize).Select(i => new InsecticideDto
            {
                Id = i.Id,
                Type = i.Type,
                //File = i.File,
                Title = i.Title,
                PublicTitle = i.PublicTitle,
                Description = i.Description
            }).ToListAsync();
        bool hasNestPage = false;
        if (result.Any())
            hasNestPage = await baseRepo.CheckIfHasNextPageAsync(expression, pageSize, pageNum);

        return new(result, hasNestPage);
    }
    public async Task<InsecticideDto> GetByIdAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Insecticide not found..");

        return await context.Insecticide.Where(i => i.Id == id && i.IsValid).Select(i => new InsecticideDto
        {
            Id = i.Id,
            Type = i.Type,
            //File = i.File,
            Title = i.Title,
            PublicTitle = i.PublicTitle,
            Description = i.Description
        }).FirstOrDefaultAsync();
    }
    public async Task RemoveAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Insecticide not found..");

        await context.Insecticide.Where(i => i.Id == id && i.IsValid).ExecuteUpdateAsync(i => i.SetProperty(i => i.IsValid, false));
    }
    public async Task<bool> CheckIfExistAsync(long id)
    => await context.Insecticide.AnyAsync(i => i.Id == id && i.IsValid);
    public async Task<bool> CheckIfExistByIdsAsync(List<long> ids)
    => await context.Insecticide.AnyAsync(l => ids.Contains(l.Id) && l.IsValid);

    #region Store

    private async Task AddInsecticidesToStore(Dictionary<long, double> addToStoreDic)
    {
        if (addToStoreDic.Count > 0)
        {
            var newInsecticides = addToStoreDic.Select(x => new InsecticideStoreModel
            {
                InsecticideId = x.Key,
                TotalQuantity = x.Value
            }).ToList();

            await context.InsecticideStore.AddRangeAsync(newInsecticides);
        }
    }
    private async Task AddInsecticideTransactionsAsync(Dictionary<long, double> dictionary, DateTime date, bool isAdd)
    {
        var transactions = dictionary.Select(x => new InsecticideTransactionModel
        {
            Date = date,
            IsAdd = isAdd,
            InsecticideId = x.Key,
            QuantityChange = x.Value
        }).ToList();

        await context.InsecticideTransaction.AddRangeAsync(transactions);
    }

    public async Task<CommonResponseDto<List<InsecticideStoreDto>>> GetAllInsecticideStoreAsync(string insecticideTitle, InsecticideType? type, DateTime? date, int pageSize, int pageNum)
    {
        Expression<Func<InsecticideStoreModel, bool>> expression = s => (string.IsNullOrEmpty(insecticideTitle) || s.Insecticide.Title.Contains(insecticideTitle))
        && (!type.HasValue || s.Insecticide.Type == type)
        && (!date.HasValue || s.CreateDate <= date)
        && s.IsValid;

        var result = await context.InsecticideStore.Where(expression)
            .Skip(pageSize * pageNum)
            .Take(pageSize)
            .Select(s => new InsecticideStoreDto
            {
                Id = s.Id,
                TotalQuantity = s.TotalQuantity,
                Insecticide = new InsecticideDto
                {
                    Id = s.Insecticide.Id,
                    Type = s.Insecticide.Type,
                    Title = s.Insecticide.Title,
                    PublicTitle = s.Insecticide.PublicTitle,
                    Description = s.Insecticide.Description
                }
            }).ToListAsync();

        bool hasNextPage = false;
        if (result.Count > 0)
            hasNextPage = await storeBaseRepo.CheckIfHasNextPageAsync(f => f.IsValid, pageSize, pageNum);

        return new CommonResponseDto<List<InsecticideStoreDto>>(result, hasNextPage);
    }

    public async Task<CommonResponseDto<List<InsecticideTransactionDto>>> GetAllInsecticideTransactionAsync(string insecticideTitle, DateTime? from, DateTime? to,
        int pageSize, int pageNum)
    {
        Expression<Func<InsecticideTransactionModel, bool>> expression = f => (string.IsNullOrEmpty(insecticideTitle) || f.Insecticide.Title.Contains(insecticideTitle))
        && (!from.HasValue || f.Date >= from)
        && (!to.HasValue || f.Date <= to)
        && f.IsValid;

        var x = await context.InsecticideTransaction.Where(expression)
            .Skip(pageNum * pageSize)
            .Take(pageSize)
            .Select(i => new InsecticideTransactionDto
            {
                Id = i.Id,
                Date = i.Date,
                IsAdd = i.IsAdd,
                QuantityChange = i.QuantityChange,
                Insecticide = new InsecticideDto
                {
                    Id = i.Insecticide.Id,
                    Type = i.Insecticide.Type,
                    Title = i.Insecticide.Title,
                    Description = i.Insecticide.Description,
                    PublicTitle = i.Insecticide.PublicTitle,
                }
            }).ToListAsync();

        bool hasNextPage = false;
        if (x.Count > 0)
            hasNextPage = await transactionBaseRepo.CheckIfHasNextPageAsync(f => f.IsValid, pageSize, pageNum);

        return new CommonResponseDto<List<InsecticideTransactionDto>>(x, hasNextPage);
    }

    public async Task UpdateStoreAsync(Dictionary<long, double> insecticidesDic, DateTime date, bool isAdd)
    {
        if (insecticidesDic.Any(x => x.Value < 0))
            throw new NotFoundException("One of quantity is less than 0..");

        var insecticideStoreModels = await context.InsecticideStore.Where(x => insecticidesDic.Keys.Contains(x.InsecticideId) && x.IsValid).ToListAsync();
        // For pulling (!isAdd), ensure all requested fertilizers exist in store
        if (!isAdd && insecticideStoreModels.Count != insecticidesDic.Count)
        {
            var missingIds = insecticidesDic.Keys.Except(insecticideStoreModels.Select(f => f.InsecticideId));
            throw new NotFoundException($"Insecticides not found in store: {string.Join(", ", missingIds)}");
        }

        // Check if pulling more than available
        if (!isAdd)
        {
            var insufficient = insecticideStoreModels
                .Where(f => f.TotalQuantity < insecticidesDic[f.InsecticideId])
                .Select(f => f.InsecticideId);

            if (insufficient.Any())
                throw new NotFoundException($"Insufficient quantity for Insecticides: {string.Join(", ", insufficient)}");
        }

        var toAddDic = insecticidesDic
            .Where(x => isAdd && !insecticideStoreModels.Any(f => f.InsecticideId == x.Key))
            .ToDictionary(x => x.Key, x => x.Value);

        var toUpdateDic = insecticidesDic
            .Where(x => insecticideStoreModels.Any(f => f.InsecticideId == x.Key))
            .ToDictionary(x => x.Key, x => x.Value);

        if (toAddDic.Count > 0)
            await AddInsecticidesToStore(toAddDic);

        foreach (var storeItem in insecticideStoreModels)
        {
            if (toUpdateDic.TryGetValue(storeItem.InsecticideId, out double quantity))
            {
                storeItem.TotalQuantity += isAdd ? quantity : -quantity;

                // Prevent negative stock
                if (storeItem.TotalQuantity < 0)
                    throw new InvalidOperationException($"Negative stock not allowed for Insecticide {storeItem.InsecticideId}");
            }
        }

        await AddInsecticideTransactionsAsync(insecticidesDic, date, isAdd);

        await context.SaveChangesAsync();
    }

    public async Task UpdateStoreForMixAsync(long mixId, double donumNum, DateTime date)
    {
        try
        {
            await dbRepo.BeginTransactionAsync();

            var mixModels = (await context.InsecticideMixDetail.Where(m => m.InsecticideMixId == mixId && m.IsValid).Include(x => x.Insecticide).ToListAsync());

            if (mixModels.Count == 0)
                throw new NotFoundException("mix not found..");

            var toUpdateStoreDic = mixModels.ToDictionary(x => x.InsecticideId,
                x => x.Insecticide.Type == InsecticideType.Powder ? x.Quantity.Value * donumNum : x.Liter * donumNum);


            await UpdateStoreAsync(toUpdateStoreDic, date, false);

            await context.InsecticideApplicableMix.AddAsync(new InsecticideApplicableMixModel
            {
                DonumCount = donumNum,
                InsecticideMixId = mixId,
                CurrentDonumCount = donumNum,
            });

            await context.SaveChangesAsync();
            await dbRepo.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await dbRepo.RollbackTransactionAsync();
            throw;
        }
    }
    #endregion
}
