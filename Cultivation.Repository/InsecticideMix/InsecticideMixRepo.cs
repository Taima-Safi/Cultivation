using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Insecticide;
using Cultivation.Repository.Base;
using Cultivation.Repository.DataBase;
using Cultivation.Repository.Insecticide;
using Cultivation.Shared.Enum;
using FourthPro.Dto.Common;
using FourthPro.Shared.Exception;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cultivation.Repository.InsecticideMix;

public class InsecticideMixRepo : IInsecticideMixRepo
{
    private readonly IDbRepo dbRepo;
    private readonly CultivationDbContext context;
    private readonly IInsecticideRepo insecticideRepo;
    private readonly IBaseRepo<InsecticideMixModel> baseRepo;

    public InsecticideMixRepo(CultivationDbContext context, IBaseRepo<InsecticideMixModel> baseRepo, IDbRepo dbRepo, IInsecticideRepo insecticideRepo)
    {
        this.dbRepo = dbRepo;
        this.context = context;
        this.baseRepo = baseRepo;
        this.insecticideRepo = insecticideRepo;
    }
    public async Task AddAsync(InsecticideMixFormDto dto)
    {
        try
        {
            await dbRepo.BeginTransactionAsync();

            if (!await insecticideRepo.CheckIfExistByIdsAsync(dto.Mixes.Select(m => m.InsecticideId).ToList()))
                throw new NotFoundException("There Insecticide not found..");

            var mixModel = await context.InsecticideMix.AddAsync(new InsecticideMixModel
            {
                Note = dto.Note,
                Title = dto.Title,
                Color = dto.Color,
            });
            await dbRepo.SaveChangesAsync();

            await context.InsecticideMixDetail.AddRangeAsync(dto.Mixes.Select(md => new InsecticideMixDetailModel
            {
                Liter = md.Liter,
                Quantity = md.Quantity,
                InsecticideId = md.InsecticideId,
                InsecticideMixId = mixModel.Entity.Id,
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

    public async Task<CommonResponseDto<List<GetInsecticideMixDto>>> GetAllAsync(string title, string note, int pageSize, int pageNum)
    {
        Expression<Func<InsecticideMixModel, bool>> expression = f =>
        (string.IsNullOrEmpty(title) || f.Title.Contains(title)) && (string.IsNullOrEmpty(note) || f.Note.Contains(note))
        && f.IsValid;

        var x = await context.InsecticideMix.Where(expression)
            .Skip(pageNum * pageSize)
            .Take(pageSize)
            .Select(i => new GetInsecticideMixDto
            {
                Id = i.Id,
                Note = i.Note,
                Title = i.Title,
                Color = i.Color,
                MixDetails = i.InsecticideMixDetails.Select(im => new InsecticideMixDetailDto
                {
                    Quantity = im.Quantity,
                    Liter = im.Liter,
                    Insecticide = new InsecticideDto
                    {
                        Id = im.Insecticide.Id,
                        Type = im.Insecticide.Type,
                        Title = im.Insecticide.Title,
                        Description = im.Insecticide.Description,
                        PublicTitle = im.Insecticide.PublicTitle,
                    }
                }).ToList()
            }).ToListAsync();

        bool hasNextPage = false;
        if (x.Count > 0)
            hasNextPage = await baseRepo.CheckIfHasNextPageAsync(f => f.IsValid, pageSize, pageNum);

        return new CommonResponseDto<List<GetInsecticideMixDto>>(x, hasNextPage);
    }
    public async Task<GetInsecticideMixDto> GetByIdAsync(long id)
    {
        if (!await baseRepo.CheckIfExistAsync(f => f.Id == id && f.IsValid))
            throw new NotFoundException("mix not found..");

        return await context.InsecticideMix.Where(f => f.Id == id && f.IsValid).Select(f => new GetInsecticideMixDto
        {
            Id = f.Id,
            Note = f.Note,
            Title = f.Title,
            Color = f.Color,
            MixDetails = f.InsecticideMixDetails.Select(im => new InsecticideMixDetailDto
            {
                Quantity = im.Quantity,
                Liter = im.Liter,
                Insecticide = new InsecticideDto
                {
                    Id = im.Insecticide.Id,
                    Type = im.Insecticide.Type,
                    Title = im.Insecticide.Title,
                    Description = im.Insecticide.Description,
                    PublicTitle = im.Insecticide.PublicTitle,
                }
            }).ToList()
        }).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(long id, string title, string note, ColorType color)
    {
        if (!await baseRepo.CheckIfExistAsync(x => x.Id == id && x.IsValid))
            throw new NotFoundException("Mix not found..");

        await context.InsecticideMix.Where(f => f.Id == id && f.IsValid).ExecuteUpdateAsync(f => f.SetProperty(f => f.Title, title)
        .SetProperty(f => f.Color, color).SetProperty(f => f.Note, note));
    }

    public async Task RemoveAsync(long id)
    {
        if (!await baseRepo.CheckIfExistAsync(x => x.Id == id && x.IsValid))
            throw new NotFoundException("Mix not found..");

        await context.InsecticideMix.Where(f => f.Id == id && f.IsValid).ExecuteUpdateAsync(f => f.SetProperty(f => f.IsValid, false));
    }
}
