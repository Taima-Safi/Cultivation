﻿using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Insecticide;
using Cultivation.Dto.InsecticideLand;
using Cultivation.Dto.Land;
using Cultivation.Repository.Base;
using Cultivation.Shared.Enum;
using FourthPro.Dto.Common;
using FourthPro.Shared.Exception;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cultivation.Repository.InsecticideLand;

public class InsecticideLandRepo : IInsecticideLandRepo
{
    private readonly CultivationDbContext context;
    private readonly IBaseRepo<InsecticideLandModel> baseRepo;

    public InsecticideLandRepo(CultivationDbContext context, IBaseRepo<InsecticideLandModel> baseRepo)
    {
        this.context = context;
        this.baseRepo = baseRepo;
    }
    public async Task<long> AddAsync(InsecticideLandFormDto dto)
    {
        string fileName = null;
        if (dto.File != null)
            fileName = FileHelper.FileHelper.UploadFile(dto.File, FileType.InsecticideLand);

        var model = await context.InsecticideLand.AddAsync(new InsecticideLandModel
        {
            Note = dto.Note,
            File = fileName,
            Date = dto.Date,
            Liter = dto.Liter,
            LandId = dto.LandId,
            Quantity = dto.Quantity,
            InsecticideId = dto.InsecticideId,
        });
        await context.SaveChangesAsync();

        return model.Entity.Id;
    }
    public async Task<CommonResponseDto<List<InsecticideLandDto>>> GetAllAsync(DateTime? date, string note, double? liter, double? quantity, long? landId, long? insecticideId
        , int pageSize, int pageNum)
    {
        Expression<Func<InsecticideLandModel, bool>> expression = il => (!insecticideId.HasValue || il.InsecticideId == insecticideId) && (!landId.HasValue || il.LandId == landId)
        && (!quantity.HasValue || il.Quantity == quantity) && (!liter.HasValue || il.Liter == liter) && (!date.HasValue || il.Date == date)
        && (string.IsNullOrEmpty(note) || il.Note.Contains(note)) && il.IsValid;

        var result = await context.InsecticideLand.Where(expression)
            .Skip(pageSize * pageNum)
            .Take(pageSize)
            .Select(il => new InsecticideLandDto
            {
                Id = il.Id,
                Date = il.Date,
                Note = il.Note,
                Liter = il.Liter,
                Quantity = il.Quantity,
                File = il.File,
                Land = new LandDto
                {
                    Id = il.Land.Id,
                    Size = il.Land.Size,
                    Title = il.Land.Title,
                },
                Insecticide = new InsecticideDto
                {
                    Id = il.Insecticide.Id,
                    Type = il.Insecticide.Type,
                    Title = il.Insecticide.Title,
                    Description = il.Insecticide.Description,
                    PublicTitle = il.Insecticide.PublicTitle,
                }
            }).ToListAsync();

        var hasNextPage = false;
        if(result.Any())
            hasNextPage = await baseRepo.CheckIfHasNextPageAsync(expression, pageSize, pageNum);    

        return new(result, hasNextPage);
    }
    public async Task<InsecticideLandDto> GetByIdAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Land not has this insecticide...");

        return await context.InsecticideLand.Where(il => il.Id == id && il.IsValid).Select(il => new InsecticideLandDto
        {
            Id = il.Id,
            Date = il.Date,
            Note = il.Note,
            Liter = il.Liter,
            Quantity = il.Quantity,
            File = il.File,
            Land = new LandDto
            {
                Id = il.Land.Id,
                Size = il.Land.Size,
                Title = il.Land.Title,
            },
            Insecticide = new InsecticideDto
            {
                Id = il.Insecticide.Id,
                Type = il.Insecticide.Type,
                Title = il.Insecticide.Title,
                Description = il.Insecticide.Description,
                PublicTitle = il.Insecticide.PublicTitle,
            }
        }).FirstOrDefaultAsync();
    }
    public async Task UpdateAsync(long id, InsecticideLandFormDto dto)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Land not has this insecticide...");

        await context.InsecticideLand.ExecuteUpdateAsync(il => il.SetProperty(il => il.Note, dto.Note).SetProperty(il => il.Quantity, dto.Quantity)
        .SetProperty(il => il.Liter, dto.Liter).SetProperty(il => il.Date, dto.Date).SetProperty(il => il.InsecticideId, dto.InsecticideId)
        .SetProperty(il => il.LandId, dto.LandId));
    }
    public async Task RemoveAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Land not has this insecticide...");

        await context.InsecticideLand.ExecuteUpdateAsync(il => il.SetProperty(il => il.IsValid, false));
    }

    public async Task<bool> CheckIfExistAsync(long id)
        => await context.InsecticideLand.AnyAsync(il => il.Id == id && il.IsValid);
}
