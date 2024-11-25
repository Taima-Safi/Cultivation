﻿using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Client;
using Cultivation.Repository.Base;
using FourthPro.Dto.Common;
using Microsoft.EntityFrameworkCore;

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
        var x = await context.Client.Where(c => (string.IsNullOrEmpty(name) || c.Name.Contains(name))
        && (!isLocal.HasValue || c.IsLocal == isLocal)
        && c.IsValid)
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
            hasNextPage = await baseRepo.CheckIfHasNextPageAsync(fl => fl.IsValid, pageSize, pageNum);

        return new CommonResponseDto<List<ClientDto>>(x, hasNextPage);
    }
}