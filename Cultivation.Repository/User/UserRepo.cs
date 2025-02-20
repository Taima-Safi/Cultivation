using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Role;
using Cultivation.Dto.User;
using Cultivation.Repository.Base;
using Cultivation.Repository.DataBase;
using Cultivation.Repository.User.Service;
using Cultivation.Shared.Enum;
using Cultivation.Shared.Exception;
using Cultivation.Shared.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Cultivation.Repository.User;

public class UserRepo : UserService, IUserRepo
{
    private readonly CultivationDbContext context;
    private readonly IConfiguration configuration;
    //private readonly TokenRepo tokenRepo;
    private readonly IDbRepo dbRepo;
    private readonly IBaseRepo<UserModel> baseRepo;
    private readonly IBaseRepo<RoleModel> roleBaseRepo;

    public UserRepo(CultivationDbContext context, IDbRepo dbRepo, IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
        IBaseRepo<UserModel> baseRepo, IBaseRepo<RoleModel> roleBaseRepo/*, TokenRepo tokenRepo*/)
        : base(configuration, httpContextAccessor)

    {
        this.context = context;
        this.configuration = configuration;
        this.dbRepo = dbRepo;
        this.baseRepo = baseRepo;
        this.roleBaseRepo = roleBaseRepo;
        // this.tokenRepo = tokenRepo;
    }
    public async Task<UserDto> LoginAsync(LoginDto dto)
    {
        var model = await context.User.Where(u => u.Email == dto.Email.Trim().ToLower() && u.IsValid).FirstOrDefaultAsync();
        if (model == null)
            throw new NotFoundException("This email not found");

        var userDto = await context.User.Where(u => u.Id == model.Id && u.IsValid).Select(model => new UserDto
        {
            Id = model.Id,
            Type = model.Type,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            Roles = model.UserRoles.Where(r => r.IsValid).Select(r => new UserRoleDto
            {
                Id = r.Id,
                Role = new RoleDto
                {
                    Id = r.Role.Id,
                    Title = r.Role.Title,
                    //DepoAccess = r.Role.DepoAccess,
                    //FullAccess = r.Role.FullAccess,
                    //OrderAccess = r.Role.OrderAccess,
                    //CuttingLandAccess = r.Role.CuttingLandAccess,
                }
            }).ToList()

        }).FirstOrDefaultAsync();

        return userDto;
    }
    public async Task CheckPasswordCorrectness(string password, string hashPassword, long userId)
    {
        if (hashPassword == null) { throw new ValidationException("PasswordHasBeenRemoved"); }

        var isMatchPassword = PasswordHelper.CheckPassword(password, hashPassword, out string newHash);
        if (isMatchPassword == PasswordVerificationResult.Failed)
            throw new ValidationException("PasswordOrEmailWrong");

        if (isMatchPassword == PasswordVerificationResult.SuccessRehashNeeded)
            await ChangePasswordAsync(newHash, userId);
    }
    public async Task ChangePasswordAsync(string newHashPassword, long id)
    => await context.User.Where(u => u.IsValid && u.Id == id)
    .ExecuteUpdateAsync(u => u.SetProperty(u => u.HashPassword, newHashPassword));

    public async Task<long> AddAsync(UserFormDto dto)
    {
        await dbRepo.BeginTransactionAsync();
        try
        {
            var user = await context.User.Where(u => u.Email == dto.Email.Trim().ToLower() && u.IsValid).FirstOrDefaultAsync();
            if (user != null)
                throw new AlreadyExistException("This email already Exist ");

            if (!PasswordHelper.CheckPasswordPolicy(dto.Password))
                throw new ValidationException("Error for PasswordPolicy");

            var hashPassword = PasswordHelper.HashPassword(dto.Password);

            var userModel = await context.User.AddAsync(new UserModel
            {
                Type = dto.Type,
                HashPassword = hashPassword,
                LastName = dto.LastName.Trim(),
                FirstName = dto.FirstName.Trim(),
                Email = dto.Email.Trim().ToLower(),
                PhoneNumber = dto.PhoneNumber.Trim(),
            });
            await dbRepo.SaveChangesAsync();
            if (dto.RoleIdsToAdd.Count > 0)
                await AddUserRolesAsync(dto.RoleIdsToAdd, userModel.Entity.Id);

            await dbRepo.CommitTransactionAsync();
            return userModel.Entity.Id;
        }
        catch (Exception)
        {
            await dbRepo.RollbackTransactionAsync();
            throw;
        }
    }
    public async Task UpdateAsync(UserFormDto dto, long id)
    {
        var currentUser = await context.User.FirstOrDefaultAsync(x => x.Id == CurrentUserId);
        if (currentUser.Type != UserType.SuperAdmin)
            throw new ValidationException("No Access..");

        if (!await baseRepo.CheckIfExistAsync(u => u.Id == id && u.IsValid))
            throw new NotFoundException("user not found..");

        //var user = await context.User.Where(u => u.Email == dto.Email.Trim().ToLower() && u.Id != id && u.IsValid).FirstOrDefaultAsync();
        //if (user != null)
        //    throw new AlreadyExistException("This email already Exist ");

        await context.User.Where(u => u.Id == id && u.IsValid).ExecuteUpdateAsync(u => u.SetProperty(x => x.FirstName, dto.FirstName.Trim().ToLower())
        .SetProperty(x => x.LastName, dto.LastName.Trim().ToLower())/*.SetProperty(x => x.Email, dto.Email.Trim().ToLower())*/.SetProperty(x => x.Type, dto.Type)
        .SetProperty(x => x.PhoneNumber, dto.PhoneNumber));

        //if there any role to update i have to delete all roles then add new ones..
        // front should send just ids which have to add 
        if (dto.RoleIdsToRemove.Count != 0)
            await RemoveUserRolesAsync(dto.RoleIdsToRemove, id);
        if (dto.RoleIdsToAdd.Count != 0)
            await AddUserRolesAsync(dto.RoleIdsToAdd, id);

    }
    public async Task RemoveAsync(long id)
    {
        if (!await baseRepo.CheckIfExistAsync(u => u.Id == id && u.IsValid))
            throw new NotFoundException("user not found..");

        var currentUser = await context.User.FirstOrDefaultAsync(x => x.Id == CurrentUserId);
        if (currentUser.Type != UserType.SuperAdmin)
            throw new ValidationException("No Access..");

        await context.User.Where(u => u.Id == id && u.IsValid).ExecuteUpdateAsync(u => u.SetProperty(x => x.IsValid, false));
    }
    #region UserRole
    public async Task AddUserRolesAsync(IEnumerable<long> roleIds, long userId)
    {
        if (!await roleBaseRepo.CheckIdsAsync(roleIds))
            throw new NotFoundException("role not found..");

        var userRolesModel = roleIds.Select(roleId => new UserRoleModel
        {
            RoleId = roleId,
            UserId = userId,
        }).ToList();

        await context.UserRole.AddRangeAsync(userRolesModel);
        await dbRepo.SaveChangesAsync();
    }
    public async Task<List<RoleModel>> GetUserRoleAsync(long userId) =>
         await context.UserRole.Where(s => s.IsValid && s.Id == userId).Select(r => r.Role).ToListAsync();
    public async Task RemoveUserRolesAsync(List<long> rolesToRemove, long userId)
    {
        if (!await baseRepo.CheckIfExistAsync(u => u.Id == userId && u.IsValid))
            throw new NotFoundException("user not found..");

        await context.UserRole.Where(ur => ur.UserId == userId && rolesToRemove.Contains(ur.RoleId)).ExecuteUpdateAsync(ur => ur.SetProperty(ur => ur.IsValid, false));
    }
    #endregion
}