using Cultivation.Dto.Role;
using Cultivation.Repository.User;
using Cultivation.Shared.Enum;
using Cultivation.Shared.Exception;
using FourthPro.Shared.Exception;
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

        bool? isAllowAnonymous = endPoint?.Metadata.Any(x => x.GetType() == typeof(AllowAnonymousAttribute));
        if (isAllowAnonymous == true)
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

        if (controllerName.Contains("UserController") || controllerName.Contains("Role"))
        {
            if (roleName != nameof(UserType.SuperAdmin) || userRoles.Any(x => x.FullAccess == false))
                throw new AccessViolationException("No Access");
        }
        else
        {
            var role = new RoleDto();
            //var userRoles = await userRepo.GetUserRoleAsync(userId);
            foreach (var userRole in userRoles)
            {
                role.FullAccess |= userRole.FullAccess;
                role.DepoAccess |= userRole.DepoAccess;
                role.OrderAccess |= userRole.OrderAccess;
                role.CuttingLandAccess |= userRole.CuttingLandAccess;
            }
            switch (controllerName)
            {
                case nameof(ControllerNames.OrderController):
                    if (!role.OrderAccess)
                        throw new AccessViolationException(" No Access");
                    break;
                case nameof(ControllerNames.CuttingLandController):
                    if (!role.CuttingLandAccess)
                        throw new AccessViolationException(" No Access");
                    break;
            }
            await next(context);
            return;
        }
    }
}
