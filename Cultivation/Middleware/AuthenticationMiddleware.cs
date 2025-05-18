using Cultivation.Repository.User;
using Cultivation.Shared.Enum;
using Cultivation.Shared.Exception;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Cultivation.Middleware;

public class AuthenticationMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endPoint = context.GetEndpoint() ??
            throw new NotFoundException("end point not found..");

        if (endPoint.DisplayName == "405 HTTP Method Not Supported")
            throw new MethodNotAllowedException("Http method not supported..");

        //bool? isAllowAnonymous = endPoint?.Metadata.Any(x => x.GetType() == typeof(AllowAnonymousAttribute));
        //if (isAllowAnonymous == true)
        //{
        //    await next(context);
        //    return;
        //}
        if (endPoint.Metadata.Any(x => x is AllowAnonymousAttribute))
        {
            await next(context);
            return;
        }
        var token = context.Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(token) || !context.User.Identity.IsAuthenticated)
            throw new UnauthorizedAccessException("UnAuthorized");

        var controllerName = context.Request.RouteValues["controller"].ToString() ?? throw new NotFoundException("endpoint not found");
        //string actionType = context.Request.RouteValues["action"].ToString() ?? throw new NotFoundException("endpoint not found");
        var userId = long.Parse(context.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).FirstOrDefault());
        var roleName = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).FirstOrDefault();
        var userRepo = context.RequestServices.GetRequiredService<IUserRepo>();
        var userRoles = await userRepo.GetUserRoleAsync(userId);


        if (roleName == nameof(UserType.SuperAdmin) || userRoles.Any(r => r.Title == RoleType.FullAccess.ToString()))
        {
            await next(context);
            return;
        }

        if (controllerName.Contains("User") || controllerName.Contains("Role"))
        {
            if (!userRoles.Any(x => x.Title == RoleType.FullAccess.ToString()) && roleName != nameof(UserType.SuperAdmin))
                throw new AccessViolationException("No Access");
        }
        //else
        //{
        //    var role = new CheckRoleDto();
        //    //var userRoles = await userRepo.GetUserRoleAsync(userId);
        //    foreach (var userRole in userRoles)
        //    {
        //        role.FullAccess = userRole.Title == RoleType.FullAccess.ToString();
        //        role.DepoAccess = userRole.Title == RoleType.DepoAccess.ToString();
        //        role.OrderAccess = userRole.Title == RoleType.OrderAccess.ToString();
        //        role.CuttingLandAccess = userRole.Title == RoleType.CuttingLandAccess.ToString();
        //        //role.DepoAccess |= userRole.DepoAccess;
        //        //role.CuttingLandAccess |= userRole.CuttingLandAccess;
        //    }
        //    if (role.FullAccess)
        //    {
        //        await next(context);
        //        return;
        //    }
        //    switch (controllerName)
        //    {
        //        case nameof(ControllerNames.Order):
        //            if (!role.OrderAccess)
        //                throw new AccessViolationException(" No Access");
        //            break;
        //        case nameof(ControllerNames.CuttingLand):
        //            if (!role.CuttingLandAccess)
        //                throw new AccessViolationException(" No Access");
        //            break;
        //    }
        //}
        // await next(context);

        var hasAccess = controllerName switch
        {
            nameof(ControllerNames.Land) => userRoles.Any(r => r.Title == RoleType.LandAccess.ToString()),
            nameof(ControllerNames.Order) => userRoles.Any(r => r.Title == RoleType.OrderAccess.ToString()),
            nameof(ControllerNames.Flower) => userRoles.Any(r => r.Title == RoleType.FlowerAccess.ToString()),
            nameof(ControllerNames.FertilizerStore) => userRoles.Any(r => r.Title == RoleType.DepoAccess.ToString()),
            nameof(ControllerNames.InsecticideStore) => userRoles.Any(r => r.Title == RoleType.DepoAccess.ToString()),
            nameof(ControllerNames.CuttingLand) => userRoles.Any(r => r.Title == RoleType.CuttingLandAccess.ToString()),
            _ => true // Allow other controllers by default
        };
        if (!hasAccess)
            throw new AccessViolationException("Access denied: Insufficient permissions.");

        await next(context);

        return;
    }
}
