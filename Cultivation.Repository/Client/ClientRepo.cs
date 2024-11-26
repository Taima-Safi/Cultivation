using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Client;
using Cultivation.Repository.Base;
using FourthPro.Dto.Common;
using FourthPro.Shared.Exception;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cultivation.Repository.Client;

public class ClientRepo : IClientRepo
{
    private readonly CultivationDbContext context;
    private readonly IBaseRepo<ClientModel> baseRepo;

    public ClientRepo(CultivationDbContext context, IBaseRepo<ClientModel> baseRepo)
    {
        this.context = context;
        this.baseRepo = baseRepo;
    }
    public async Task<long> AddAsync(ClientFormDto dto)
    {
        var model = await context.Client.AddAsync(new ClientModel
        {
            Name = dto.Name,
            IsLocal = dto.IsLocal,
            PhoneNumber = dto.PhoneNumber,
            CodePhoneNumber = dto.CodePhoneNumber,
        });
        await context.SaveChangesAsync();
        return model.Entity.Id;
    }
    public async Task<CommonResponseDto<List<ClientDto>>> GetAllAsync(bool? isLocal, string name, int pageSize, int pageNum)
    {
        Expression<Func<ClientModel, bool>> expression = c => (string.IsNullOrEmpty(name) || c.Name.Contains(name))
        && (!isLocal.HasValue || c.IsLocal == isLocal)
        && c.IsValid;

        var x = await context.Client.Where(expression)
                    .Skip(pageNum * pageSize)
                    .Take(pageSize).Select(c => new ClientDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        IsLocal = c.IsLocal,
                        PhoneNumber = c.PhoneNumber,
                        CodePhoneNumber = c.CodePhoneNumber
                    }).ToListAsync();

        bool hasNextPage = false;
        if (x.Count > 0)
            hasNextPage = await baseRepo.CheckIfHasNextPageAsync(expression, pageSize, pageNum);

        return new CommonResponseDto<List<ClientDto>>(x, hasNextPage);
    }

    public async Task UpdateAsync(long id, string phone, string codePhone, string name)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Client not found..");

        await context.Client.Where(c => c.Id == id && c.IsValid).ExecuteUpdateAsync(c => c.SetProperty(c => c.Name, name)
        .SetProperty(c => c.PhoneNumber, phone).SetProperty(c => c.CodePhoneNumber, codePhone));
    }

    public async Task RemoveAsync(long id)
     => await context.Client.Where(c => c.Id == id && c.IsValid).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsValid, false));
    public async Task<bool> CheckIfExistAsync(long id)
     => await context.Client.Where(c => c.Id == id && c.IsValid).AnyAsync();
}
