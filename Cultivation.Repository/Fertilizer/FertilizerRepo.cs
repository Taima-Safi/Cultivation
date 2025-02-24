using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Fertilizer;
using Cultivation.Repository.Base;
using Cultivation.Dto.Common;
using Cultivation.Shared.Exception;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cultivation.Repository.Fertilizer;

public class FertilizerRepo : IFertilizerRepo
{
    private readonly IBaseRepo<FertilizerModel> baseRepo;
    private readonly CultivationDbContext context;
    public FertilizerRepo(CultivationDbContext context, IBaseRepo<FertilizerModel> baseRepo)
    {
        this.context = context;
        this.baseRepo = baseRepo;
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
            // Price = dto.Price,
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
                //Price = f.Price,
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
            //Price = f.Price,
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
        .SetProperty(f => f.Description, dto.Description)
        /*.SetProperty(f => f.Price, dto.Price)*/);
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
}
