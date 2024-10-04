using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Color;
using Cultivation.Repository.Base;
using FourthPro.Dto.Common;
using FourthPro.Shared.Exception;
using Microsoft.EntityFrameworkCore;

namespace Cultivation.Repository.Color;

public class ColorRepo : IColorRepo
{
    private readonly CultivationDbContext context;
    private readonly IBaseRepo<ColorModel> baseRepo;

    public ColorRepo(CultivationDbContext context, IBaseRepo<ColorModel> baseRepo)
    {
        this.context = context;
        this.baseRepo = baseRepo;
    }

    public async Task<long> AddAsync(string title, string code)
    {
        var color = await context.Color.AddAsync(new ColorModel
        {
            Code = code,
            Title = title
        });
        await context.SaveChangesAsync();
        return color.Entity.Id;
    }
    public async Task<CommonResponseDto<List<ColorDto>>> GetAllAsync(string title, string code, int pageSize, int pageNum)
    {
        var x = await context.Color.Where(c => (string.IsNullOrEmpty(title) || c.Title.Contains(title))
        && (string.IsNullOrEmpty(code) || c.Title.Contains(code))
        && c.IsValid)
            .Skip(pageNum * pageSize)
            .Take(pageSize).Select(c => new ColorDto
            {
                Id = c.Id,
                Code = c.Code,
                Title = c.Title
            }).ToListAsync();

        bool hasNextPage = false;
        if (x.Count > 0)
            hasNextPage = await baseRepo.CheckIfHasNextPageAsync(fl => fl.IsValid, pageSize, pageNum);

        return new CommonResponseDto<List<ColorDto>>(x, hasNextPage);
    }
    public async Task<ColorDto> GetByIdAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Color not found..");

        return await context.Color.Where(c => c.Id == id && c.IsValid).Select(c => new ColorDto
        {
            Id = c.Id,
            Code = c.Code,
            Title = c.Title
        }).FirstOrDefaultAsync();
    }
    public async Task UpdateAsync(long id, string title, string code)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Color not found..");

        await context.Color.Where(c => c.Id == id && c.IsValid).ExecuteUpdateAsync(c => c.SetProperty(c => c.Code, code).SetProperty(c => c.Title, title));
    }

    public async Task RemoveAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Color not found..");

        await context.Color.Where(c => c.Id == id && c.IsValid).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsValid, false));
    }

    public async Task<bool> CheckIfExistAsync(long id)
     => await context.Color.Where(c => c.Id == id && c.IsValid).AnyAsync();
}
