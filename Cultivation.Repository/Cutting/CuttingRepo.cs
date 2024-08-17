using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Color;
using Cultivation.Dto.Cutting;
using FourthPro.Shared.Exception;
using Microsoft.EntityFrameworkCore;

namespace Cultivation.Repository.Cutting;

public class CuttingRepo : ICuttingRepo
{
    private readonly CultivationDbContext context;
    public CuttingRepo(CultivationDbContext context)
    {
        this.context = context;
    }
    public async Task<long> AddAsync(string title, string type, int age)
    {
        var cutting = await context.Cutting.AddAsync(new CuttingModel
        {
            Age = age,
            Type = type,
            Title = title
        });
        await context.SaveChangesAsync();
        return cutting.Entity.Id;
    }
    public async Task<List<CuttingDto>> GetAllAsync(string title, string type, int? age)
    {
        return await context.Cutting.Where(c => (string.IsNullOrEmpty(title) || c.Title.Contains(title))
        && (string.IsNullOrEmpty(type) || c.Type.Contains(type))
        && (!age.HasValue || c.Age == age)
        && c.IsValid).Select(c => new CuttingDto
        {
            Id = c.Id,
            Age = c.Age,
            Type = c.Type,
            Title = c.Title
        }).ToListAsync();
    }
    public async Task<CuttingDto> GetByIdAsync(long id)
    {
        if (!await CheckCuttingIfExistAsync(id))
            throw new NotFoundException("Cutting not found..");

        return await context.Cutting.Where(c => c.Id == id && c.IsValid).Select(c => new CuttingDto
        {
            Id = c.Id,
            Age = c.Age,
            Type = c.Type,
            Title = c.Title
        }).FirstOrDefaultAsync();
    }
    public async Task UpdateAsync(long id, string title, string type, int age)
    {
        if (!await CheckCuttingIfExistAsync(id))
            throw new NotFoundException("Cutting not found..");

        await context.Cutting.Where(c => c.Id == id && c.IsValid).ExecuteUpdateAsync(c => c.SetProperty(c => c.Age, age)
            .SetProperty(c => c.Type, type).SetProperty(c => c.Title, title));
    }

    public async Task RemoveAsync(long id)
    {
        if (!await CheckCuttingIfExistAsync(id))
            throw new NotFoundException("Cutting not found..");

        await context.Cutting.Where(c => c.Id == id && c.IsValid).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsValid, false));
    }

    public async Task<bool> CheckCuttingIfExistAsync(long id)
        => await context.Cutting.Where(c => c.Id == id && c.IsValid).AnyAsync();

    #region CuttingColor
    public async Task<long> AddCuttingColorAsync(CuttingColorFormDto dto)
    {
        var cuttingColor = await context.CuttingColor.AddAsync(new CuttingColorModel
        {
            Code = dto.Code,
            ColorId = dto.ColorId,
            CuttingId = dto.CuttingId,
        });
        await context.SaveChangesAsync();
        return cuttingColor.Entity.Id;
    }
    public async Task<List<CuttingColorDto>> GetAllCuttingColorAsync(string code)
    {
        return await context.CuttingColor.Where(c => (string.IsNullOrEmpty(code) || c.Code.Contains(code)) && c.IsValid)
            .Select(c => new CuttingColorDto
            {
                Id = c.Id,
                Code = c.Code,
                Color = new ColorDto
                {
                    Id = c.Color.Id,
                    Code = c.Color.Code,
                    Title = c.Color.Title
                },
                Cutting = new CuttingDto
                {
                    Id = c.Cutting.Id,
                    Age = c.Cutting.Age,
                    Type = c.Cutting.Type,
                    Title = c.Cutting.Title
                }
            }).ToListAsync();
    }
    public async Task<CuttingColorDto> GetCuttingColorByIdAsync(long id)
    {
        if (!await CheckCuttingColorIfExistAsync(id))
            throw new NotFoundException("color for this cutting not found..");

        return await context.CuttingColor.Where(c => c.Id == id && c.IsValid).Select(c => new CuttingColorDto
        {
            Id = c.Id,
            Code = c.Code,
            Color = new ColorDto
            {
                Id = c.Color.Id,
                Code = c.Color.Code,
                Title = c.Color.Title
            },
            Cutting = new CuttingDto
            {
                Id = c.Cutting.Id,
                Age = c.Cutting.Age,
                Type = c.Cutting.Type,
                Title = c.Cutting.Title
            }
        }).FirstOrDefaultAsync();
    }
    public async Task UpdateCuttingColorAsync(long id, CuttingColorFormDto dto)
    {
        if (!await CheckCuttingColorIfExistAsync(id))
            throw new NotFoundException("Color for cutting not found..");

        await context.CuttingColor.Where(c => c.Id == id && c.IsValid).ExecuteUpdateAsync(c => c.SetProperty(c => c.Code, dto.Code)
        .SetProperty(c => c.ColorId, dto.ColorId).SetProperty(c => c.CuttingId, dto.CuttingId));
    }

    public async Task RemoveCuttingColorAsync(long id)
    {
        if (!await CheckCuttingColorIfExistAsync(id))
            throw new NotFoundException("Color for cutting not found..");

        await context.CuttingColor.Where(c => c.Id == id && c.IsValid).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsValid, false));
    }

    public async Task<bool> CheckCuttingColorIfExistAsync(long id)
        => await context.CuttingColor.Where(c => c.Id == id && c.IsValid).AnyAsync();
    #endregion
}
