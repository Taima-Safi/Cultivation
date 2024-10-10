using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Insecticide;
using Cultivation.Repository.Base;
using Cultivation.Shared.Enum;
using FourthPro.Dto.Common;
using FourthPro.Shared.Exception;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cultivation.Repository.Insecticide;

public class InsecticideRepo : IInsecticideRepo
{
    private readonly CultivationDbContext context;
    private readonly IBaseRepo<InsecticideModel> baseRepo;
    public InsecticideRepo(CultivationDbContext context, IBaseRepo<InsecticideModel> baseRepo)
    {
        this.context = context;
        this.baseRepo = baseRepo;
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
}
