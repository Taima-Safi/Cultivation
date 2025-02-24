﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Cultivation.Repository.User.Service;

public class UserService
{
    protected readonly IConfiguration configuration;
    protected readonly IHttpContextAccessor httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.configuration = configuration;
    }

    public UserService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        this.configuration = configuration;
        this.httpContextAccessor = httpContextAccessor;
    }

    public long CurrentUserId
    {
        get
        {
            return GetCurrentUserId();
        }
    }
    public string CurrentJwtId
    {
        get
        {
            return GetCurrentJwtId();
        }
    }
    public string GetCurrentJwtId()
    {
        var claim = httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == JwtRegisteredClaimNames.Jti);
        if (claim == null)
            return "";
        return claim.ToString().Split("jti: ").ElementAt(1);
    }
    public long GetCurrentUserId()
    {
        var claim = httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
        if (claim == null)
            return -1;
        return long.Parse(claim.Value);
    }
}
