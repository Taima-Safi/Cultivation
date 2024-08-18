﻿using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Color;
using Cultivation.Dto.Cutting;
using Cultivation.Dto.CuttingLand;
using Cultivation.Dto.Land;
using Cultivation.Repository.Cutting;
using Cultivation.Repository.Land;
using FourthPro.Shared.Exception;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cultivation.Repository.CuttingLand;

public class CuttingLandRepo : ICuttingLandRepo
{
    private readonly CultivationDbContext context;
    private readonly ILandRepo landRepo;
    private readonly ICuttingRepo cuttingRepo;

    public CuttingLandRepo(CultivationDbContext context, ILandRepo landRepo, ICuttingRepo cuttingRepo)
    {
        this.context = context;
        this.landRepo = landRepo;
        this.cuttingRepo = cuttingRepo;
    }

    public async Task<long> AddAsync(CuttingLandFormDto dto)
    {
        if (!await landRepo.CheckIfExistAsync(dto.LandId))
            throw new NotFoundException("Land not found..");

        if (!await cuttingRepo.CheckCuttingColorIfExistAsync(dto.CuttingColorId))
            throw new NotFoundException("Cutting not found..");

        var x = await context.CuttingLand.AddAsync(new CuttingLandModel
        {
            Date = dto.Date,
            LandId = dto.LandId,
            Quantity = dto.Quantity,
            CuttingColorId = dto.CuttingColorId
        });
        await context.SaveChangesAsync();
        return x.Entity.Id;
    }

    public async Task<List<CuttingLandDto>> GetAllAsync(DateTime? date)
    {
        Expression<Func<CuttingLandModel, bool>> expression = cl => (!date.HasValue || cl.Date.Date == date) && cl.IsValid;

        return await context.CuttingLand.Where(expression).Select(cl => new CuttingLandDto
        {
            Id = cl.Id,
            Date = cl.Date,
            Quantity = cl.Quantity,
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
                Title = cl.Land.Title
            },
            CuttingColor = new CuttingColorDto
            {
                Id = cl.CuttingColor.Id,
                Code = cl.CuttingColor.Code,
                Color = new ColorDto
                {
                    Id = cl.CuttingColor.Color.Id,
                    Title = cl.CuttingColor.Color.Title
                },
                Cutting = new CuttingDto
                {
                    Id = cl.CuttingColor.Cutting.Id,
                    Type = cl.CuttingColor.Cutting.Type,
                    Title = cl.CuttingColor.Cutting.Title
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

        await context.CuttingLand.Where(cl => cl.Id == id && cl.IsValid).ExecuteUpdateAsync(cl => cl.SetProperty(cl => cl.IsValid, false));
    }
    public async Task<bool> CheckIfExistAsync(long id)
    => await context.CuttingLand.Where(cl => cl.Id == id && cl.IsValid).AnyAsync();
}