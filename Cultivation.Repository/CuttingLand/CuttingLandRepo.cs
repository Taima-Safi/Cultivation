﻿using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Color;
using Cultivation.Dto.Cutting;
using Cultivation.Dto.CuttingLand;
using Cultivation.Dto.Land;
using Cultivation.Repository.Base;
using Cultivation.Repository.Cutting;
using Cultivation.Repository.Land;
using Cultivation.Dto.Common;
using Cultivation.Shared.Exception;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cultivation.Repository.CuttingLand;

public class CuttingLandRepo : ICuttingLandRepo
{
    private readonly CultivationDbContext context;
    private readonly IBaseRepo<CuttingLandModel> baseRepo;
    private readonly IBaseRepo<CuttingColorModel> baseCuttingColorRepo;
    private readonly ILandRepo landRepo;
    private readonly ICuttingRepo cuttingRepo;

    public CuttingLandRepo(CultivationDbContext context, ILandRepo landRepo, ICuttingRepo cuttingRepo, IBaseRepo<CuttingLandModel> baseRepo, IBaseRepo<CuttingColorModel> baseCuttingColorRepo)
    {
        this.context = context;
        this.landRepo = landRepo;
        this.cuttingRepo = cuttingRepo;
        this.baseRepo = baseRepo;
        this.baseCuttingColorRepo = baseCuttingColorRepo;
    }

    public async Task AddAsync(CuttingLandFormDto dto)
    {
        if (!await landRepo.CheckIfExistAsync(dto.LandId))
            throw new NotFoundException("Land not found..");

        if (!await baseCuttingColorRepo.CheckIfExistAsync(c => dto.Cuttings.Select(x => x.CuttingColorId).Contains(c.Id)))
            throw new NotFoundException("One of cuttings not found..");
        List<CuttingLandModel> cuttingLands = new List<CuttingLandModel>();
        foreach (var cutting in dto.Cuttings)
        {
            cuttingLands.Add(new CuttingLandModel
            {
                Date = dto.Date,
                IsActive = true,
                LandId = dto.LandId,
                Quantity = cutting.Quantity,
                CuttingColorId = cutting.CuttingColorId,
            });
        }
        await context.CuttingLand.AddRangeAsync(cuttingLands);
        await context.SaveChangesAsync();
    }

    public async Task UpdateIsActiveAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Cutting not found..");

        await context.CuttingLand.Where(cl => cl.Id == id && cl.IsValid).ExecuteUpdateAsync(cl => cl.SetProperty(cl => cl.IsActive, false));
    }

    public async Task<List<long>> GetActiveCuttingLandIdsAsync(List<long> landIds)
    => await context.CuttingLand.Where(c => c.IsValid && c.IsActive && landIds.Contains(c.LandId)).Select(c => c.Id).ToListAsync();
    public async Task<long> GetCuttingLandIdAsync(long landId)
    => await context.CuttingLand.Where(c => c.IsValid && c.IsActive && c.LandId == landId).Select(c => c.Id).FirstOrDefaultAsync();


    public async Task<CommonResponseDto<List<CuttingLandDto>>> GetAllAsync(DateTime? date, int pageSize = 10, int pageNum = 0)
    {
        Expression<Func<CuttingLandModel, bool>> expression = cl => (date.HasValue ? cl.Date.Date == date : cl.IsActive) && cl.IsValid;

        var result = await context.CuttingLand
            .Where(expression)
            .Skip(pageNum * pageSize)
            .Take(pageSize)
            .Select(cl => new CuttingLandDto
            {
                Id = cl.Id,
                Date = cl.Date,
                Quantity = cl.Quantity,
                IsActive = cl.IsActive,
                Land = new LandDto
                {
                    Id = cl.Land.Id,
                    Title = cl.Land.Title
                },
                CuttingColor = new CuttingColorDto
                {
                    Id = cl.CuttingColor.Id,
                    Code = cl.CuttingColor.Code,
                }
            }).ToListAsync();

        bool hasNestPage = false;
        if (result.Count > 0)
            hasNestPage = await baseRepo.CheckIfHasNextPageAsync(expression, pageSize, pageNum);

        return new(result, hasNestPage);
    }

    public async Task<CuttingLandDto> GetByIdAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Cutting for this land not found..");

        return await context.CuttingLand.Where(cl => cl.Id == id && cl.IsValid).Select(cl => new CuttingLandDto
        {
            Id = cl.Id,
            Date = cl.Date,
            Quantity = cl.Quantity,
            Land = new LandDto
            {
                Id = cl.Land.Id,
                Size = cl.Land.Size,
                Title = cl.Land.Title,
                ParentId = cl.Land.ParentId,
                Location = cl.Land.Location,
            },
            CuttingColor = new CuttingColorDto
            {
                Id = cl.CuttingColor.Id,
                Code = cl.CuttingColor.Code,
                Color = new ColorDto
                {
                    Id = cl.CuttingColor.Color.Id,
                    Code = cl.CuttingColor.Color.Code,
                    Title = cl.CuttingColor.Color.Title,
                },
                Cutting = new CuttingDto
                {
                    Id = cl.CuttingColor.Cutting.Id,
                    Type = cl.CuttingColor.Cutting.Type,
                    Title = cl.CuttingColor.Cutting.Title,
                }
            }
        }).FirstOrDefaultAsync();
    }
    public async Task UpdateAsync(long id, DateTime date, long quantity)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Cutting for this land not found..");

        await context.CuttingLand.Where(cl => cl.Id == id && cl.IsValid).ExecuteUpdateAsync(cl => cl.SetProperty(cl => cl.Date, date)
        .SetProperty(cl => cl.Quantity, quantity));
    }
    public async Task RemoveAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Cutting for this land not found..");

        await context.CuttingLand.Where(cl => cl.Id == id && cl.IsValid).ExecuteUpdateAsync(cl => cl.SetProperty(cl => cl.IsValid, false).SetProperty(cl => cl.IsActive, false));
    }
    public async Task<bool> CheckIfExistAsync(long id)
    => await context.CuttingLand.Where(cl => cl.Id == id && cl.IsValid).AnyAsync();

    public async Task<bool> CheckIfExistByIdsAsync(List<long> ids)
    => await context.CuttingLand.AnyAsync(l => ids.Contains(l.Id) && l.IsValid);
}
