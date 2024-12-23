using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Fertilizer;
using Cultivation.Repository.Base;
using Cultivation.Repository.DataBase;
using Cultivation.Repository.Fertilizer;
using Cultivation.Shared.Enum;
using FourthPro.Dto.Common;
using FourthPro.Shared.Exception;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cultivation.Repository.FertilizerMix;

public class FertilizerMixRepo : IFertilizerMixRepo
{
    private readonly CultivationDbContext context;
    private readonly IBaseRepo<FertilizerMixModel> baseRepo;
    private readonly IDbRepo dbRepo;
    private readonly IFertilizerRepo fertilizerRepo;

    public FertilizerMixRepo(CultivationDbContext context, IBaseRepo<FertilizerMixModel> baseRepo, IDbRepo dbRepo, IFertilizerRepo fertilizerRepo)
    {
        this.context = context;
        this.baseRepo = baseRepo;
        this.dbRepo = dbRepo;
        this.fertilizerRepo = fertilizerRepo;
    }

    public async Task AddAsync(FertilizerMixFormDto dto)
    {
        try
        {
            await dbRepo.BeginTransactionAsync();

            if (!await fertilizerRepo.CheckIfExistByIdsAsync(dto.Mixes.Select(m => m.FertilizerId).ToList()))
                throw new NotFoundException("There Fertilizer not found..");

            var mixModel = await context.FertilizerMix.AddAsync(new FertilizerMixModel
            {
                Type = dto.Type,
                Title = dto.Title,
            });
            await dbRepo.SaveChangesAsync();

            await context.FertilizerMixDetail.AddRangeAsync(dto.Mixes.Select(md => new FertilizerMixDetailModel
            {
                Quantity = md.Quantity,
                FertilizerId = md.FertilizerId,
                FertilizerMixId = mixModel.Entity.Id,
            }));

            await dbRepo.SaveChangesAsync();
            await dbRepo.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await dbRepo.RollbackTransactionAsync();
            throw;
        }
    }
    public async Task<CommonResponseDto<List<GetFertilizerMixDto>>> GetAllAsync(string title, int pageSize, int pageNum)
    {
        Expression<Func<FertilizerMixModel, bool>> expression = f =>
        (string.IsNullOrEmpty(title) || f.Title.Contains(title))
        && f.IsValid;

        var x = await context.FertilizerMix.Where(expression)
            .Skip(pageNum * pageSize)
            .Take(pageSize)
            .Select(f => new GetFertilizerMixDto
            {
                Id = f.Id,
                Type = f.Type,
                Title = f.Title,
                MixDetails = f.FertilizerMixDetails.Select(fm => new FertilizerMixDetailDto
                {
                    Quantity = fm.Quantity,
                    Fertilizer = new FertilizerDto
                    {
                        Id = fm.Fertilizer.Id,
                        NPK = fm.Fertilizer.NPK,
                        Title = fm.Fertilizer.Title,
                        Description = fm.Fertilizer.Description,
                        PublicTitle = fm.Fertilizer.PublicTitle,
                    }
                }).ToList()
            }).ToListAsync();

        bool hasNextPage = false;
        if (x.Count > 0)
            hasNextPage = await baseRepo.CheckIfHasNextPageAsync(f => f.IsValid, pageSize, pageNum);

        return new CommonResponseDto<List<GetFertilizerMixDto>>(x, hasNextPage);
    }
    public async Task<GetFertilizerMixDto> GetByIdAsync(long id)
    {
        if (!await baseRepo.CheckIfExistAsync(f => f.Id == id && f.IsValid))
            throw new NotFoundException("mix not found..");

        return await context.FertilizerMix.Where(f => f.Id == id && f.IsValid).Select(f => new GetFertilizerMixDto
        {
            Id = f.Id,
            Type = f.Type,
            Title = f.Title,
            MixDetails = f.FertilizerMixDetails.Select(fm => new FertilizerMixDetailDto
            {
                Quantity = fm.Quantity,
                Fertilizer = new FertilizerDto
                {
                    Id = fm.Fertilizer.Id,
                    NPK = fm.Fertilizer.NPK,
                    Title = fm.Fertilizer.Title,
                    Description = fm.Fertilizer.Description,
                    PublicTitle = fm.Fertilizer.PublicTitle,
                }
            }).ToList()
        }).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(long id, string title, FertilizerType type)
    {
        if (!await baseRepo.CheckIfExistAsync(x => x.Id == id && x.IsValid))
            throw new NotFoundException("Mix not found..");

        await context.FertilizerMix.Where(f => f.Id == id && f.IsValid).ExecuteUpdateAsync(f => f.SetProperty(f => f.Title, title).SetProperty(f => f.Type, type));
    }

    public async Task RemoveAsync(long id)
    {
        if (!await baseRepo.CheckIfExistAsync(x => x.Id == id && x.IsValid))
            throw new NotFoundException("Mix not found..");

        await context.FertilizerMix.Where(f => f.Id == id && f.IsValid).ExecuteUpdateAsync(f => f.SetProperty(f => f.IsValid, false));
    }

}
