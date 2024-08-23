﻿using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Fertilizer;
using Cultivation.Dto.FertilizerLand;
using Cultivation.Dto.Land;
using Cultivation.Repository.Base;
using Cultivation.Repository.Fertilizer;
using Cultivation.Repository.Land;
using Cultivation.Shared.Enum;
using FourthPro.Dto.Common;
using FourthPro.Shared.Exception;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cultivation.Repository.FertilizerLand;

public class FertilizerLandRepo : IFertilizerLandRepo
{
    private readonly CultivationDbContext context;
    private readonly IFertilizerRepo fertilizerRepo;
    private readonly ILandRepo landRepo;
    private readonly IBaseRepo<FertilizerLandModel> baseRepo;

    public FertilizerLandRepo(CultivationDbContext context, ILandRepo landRepo, IFertilizerRepo fertilizerRepo, IBaseRepo<FertilizerLandModel> baseRepo)
    {
        this.context = context;
        this.landRepo = landRepo;
        this.fertilizerRepo = fertilizerRepo;
        this.baseRepo = baseRepo;
    }
    public async Task AddAsync(FertilizerLandFormDto dto)
    {
        List<FertilizerLandModel> models = [];
        foreach (var landId in dto.LandIds)
        {
            models.AddRange(dto.Mixes.Select(f => new FertilizerLandModel
            {
                Date = dto.Date,
                LandId = landId,
                Type = dto.Type,
                Quantity = f.Quantity,
                FertilizerId = f.FertilizerId,
            }).ToList());
        }
        await context.FertilizerLand.AddRangeAsync(models);
        await context.SaveChangesAsync();
    }
    public async Task<CommonResponseDto<List<FertilizerLandDto>>> GetAllAsync(int pageSize, int pageNum)
    {
        var result = await context.FertilizerLand.Where(fl => fl.IsValid)
            .OrderByDescending(fl => fl.Date)
            .OrderByDescending(fl => fl.LandId)
            .Skip(pageNum * pageSize)
            .Take(pageSize)
            .Select(fl => new FertilizerLandDto
            {
                Id = fl.Id,
                Date = fl.Date,
                Type = fl.Type,
                Quantity = fl.Quantity,
                Fertilizer = new FertilizerDto
                {
                    Id = fl.Fertilizer.Id,
                    NPK = fl.Fertilizer.NPK,
                    PublicTitle = fl.Fertilizer.PublicTitle
                },
                Land = new LandDto
                {
                    Id = fl.Land.Id,
                    Title = fl.Land.Title,
                }
            }).ToListAsync();

        bool hasNextPage = false;
        if (result.Count > 0)
            hasNextPage = await baseRepo.CheckIfHasNextPageAsync(fl => fl.IsValid, pageSize, pageNum);

        return new CommonResponseDto<List<FertilizerLandDto>>(result, hasNextPage);
    }
    public async Task<CommonResponseDto<List<FertilizerLandDto>>> GetFertilizersLandAsync(long landId, DateTime? from, DateTime? to, int pageSize, int pageNum)
    {
        if (!await landRepo.CheckIfExistAsync(landId))
            throw new NotFoundException("Land not found..");

        Expression<Func<FertilizerLandModel, bool>> expression = fl => fl.LandId == landId && (!from.HasValue || fl.Date.Date >= from)
        && (!to.HasValue || fl.Date.Date <= to) && fl.IsValid;
        var result = await context.FertilizerLand.Where(expression)
            .Skip(pageNum * pageSize)
            .Take(pageSize)
            .Select(fl => new FertilizerLandDto
            {
                Id = fl.Id,
                Date = fl.Date,
                Type = fl.Type,
                Quantity = fl.Quantity,
                Fertilizer = new FertilizerDto
                {
                    Id = fl.Fertilizer.Id,
                    NPK = fl.Fertilizer.NPK,
                    PublicTitle = fl.Fertilizer.PublicTitle
                },
                Land = new LandDto
                {
                    Id = fl.Land.Id,
                    Title = fl.Land.Title,
                }
            }).ToListAsync();

        bool hasNextPage = false;
        if (result.Count > 0)
            hasNextPage = await baseRepo.CheckIfHasNextPageAsync(fl => fl.IsValid, pageSize, pageNum);

        return new CommonResponseDto<List<FertilizerLandDto>>(result, hasNextPage);
    }
    public async Task<FertilizerLandDto> GetByIdAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Land not has this fertilizer..");

        return await context.FertilizerLand.Where(fl => fl.IsValid)
            .Select(fl => new FertilizerLandDto
            {
                Id = fl.Id,
                Date = fl.Date,
                Type = fl.Type,
                Quantity = fl.Quantity,
                Fertilizer = new FertilizerDto
                {
                    Id = fl.Fertilizer.Id,
                    NPK = fl.Fertilizer.NPK,
                    PublicTitle = fl.Fertilizer.PublicTitle
                },
                Land = new LandDto
                {
                    Id = fl.Land.Id,
                    Title = fl.Land.Title,
                }
            }).FirstOrDefaultAsync();
    }
    public async Task UpdateAsync(long id, double? quantity, DateTime? date, FertilizerType? type, long? landId, long? fertilizerId)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Land not has this fertilizer..");

        if (!await landRepo.CheckIfExistAsync(landId.Value))
            throw new NotFoundException("Land not found..");

        if (!await fertilizerRepo.CheckIfExistAsync(fertilizerId.Value))
            throw new NotFoundException("Fertilizer not has this fertilizer..");

        await context.FertilizerLand.Where(fl => fl.Id == id && fl.IsValid).ExecuteUpdateAsync(fl => fl.SetProperty(fl => fl.Type, type)
        .SetProperty(fl => fl.Quantity, quantity).SetProperty(fl => fl.Date, date));
    }
    public async Task RemoveAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Land not has this fertilizer..");

        await context.FertilizerLand.Where(fl => fl.Id == id && fl.IsValid).ExecuteUpdateAsync(fl => fl.SetProperty(fl => fl.IsValid, false));
    }

    public async Task<bool> CheckIfExistAsync(long id)
        => await context.FertilizerLand.Where(fl => fl.Id == id && fl.IsValid).AnyAsync();
}