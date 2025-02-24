using Cultivation.Database.Context;
using Cultivation.Repository.Base;
using Cultivation.Repository.Client;
using Cultivation.Repository.Color;
using Cultivation.Repository.Cutting;
using Cultivation.Repository.CuttingLand;
using Cultivation.Repository.DataBase;
using Cultivation.Repository.Fertilizer;
using Cultivation.Repository.FertilizerLand;
using Cultivation.Repository.FertilizerMix;
using Cultivation.Repository.File;
using Cultivation.Repository.Flower;
using Cultivation.Repository.Insecticide;
using Cultivation.Repository.InsecticideLand;
using Cultivation.Repository.InsecticideMix;
using Cultivation.Repository.Land;
using Cultivation.Repository.Order;
using Cultivation.Repository.Role;
using Cultivation.Repository.Token;
using Cultivation.Repository.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Cultivation.Configuration;

public static class ServiceConfiguration
{
    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
     => services.AddAuthentication(options =>
     {
         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
         options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
     })
     .AddJwtBearer(options =>
     {
         options.SaveToken = true;
         options.RequireHttpsMetadata = false;
         options.Events = new JwtBearerEvents
         {
             OnChallenge = context =>
             {
                 context.Response.StatusCode = 401;
                 return Task.CompletedTask;
             }
         };
         options.TokenValidationParameters = new TokenValidationParameters
         {
             ValidateIssuer = true,
             ValidateAudience = true,
             ValidAudience = configuration.GetSection("JwtConfig:validAudience").Value,
             ValidIssuer = configuration.GetSection("JwtConfig:validIssuer").Value,
             ValidateLifetime = true,
             ValidateIssuerSigningKey = true,
             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:secret"])),
             ClockSkew = TimeSpan.Zero
         };
     });
    public static void ConfigureSwagger(this IServiceCollection services)
     => services.AddSwaggerGen(c =>
     {
         c.SwaggerDoc("v1", new OpenApiInfo
         {
             Title = Environment.GetEnvironmentVariable("ASPNETCORE_SWAGGER_TITLE"),
             Version = "v1",
             Description = "Cultivation API Services.",
             Contact = new OpenApiContact
             {
                 Name = "Coursing"
             },
         });
         c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
         c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
         {
             Name = "Authorization",
             Type = SecuritySchemeType.ApiKey,
             Scheme = "Bearer",
             BearerFormat = "JWT",
             In = ParameterLocation.Header,
             Description = "JWT Authorization header using the Bearer scheme."
         });
         c.AddSecurityRequirement(new OpenApiSecurityRequirement
         {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
         });
     });

    public static void ConfigureRepos(this IServiceCollection services)
        => services.AddScoped<IDbRepo, DbRepo>()
                   .AddScoped<ILandRepo, LandRepo>()
                   .AddScoped<IUserRepo, UserRepo>()
                   .AddScoped<IRoleRepo, RoleRepo>()
                   .AddScoped<ITokenRepo, TokenRepo>(sp =>
                   {
                       var configuration = sp.GetRequiredService<IConfiguration>();
                       var jwtSettings = configuration.GetSection("JwtSettings");

                       return new TokenRepo(
                           sp.GetRequiredService<CultivationDbContext>(),
                           jwtSettings["DurationInHours"],
                           jwtSettings["Audience"],
                           jwtSettings["Issuer"],
                           jwtSettings["Key"],
                           sp.GetRequiredService<IDbRepo>(),
                           sp.GetRequiredService<IHttpContextAccessor>(),
                           configuration
                       );
                   })
                   .AddScoped<IOrderRepo, OrderRepo>()
                   .AddScoped<IColorRepo, ColorRepo>()
                   .AddScoped<IClientRepo, ClientRepo>()
                   .AddScoped<IFlowerRepo, FlowerRepo>()
                   .AddScoped<ICuttingRepo, CuttingRepo>()
                   .AddScoped<IFertilizerRepo, FertilizerRepo>()
                   .AddScoped<ICuttingLandRepo, CuttingLandRepo>()
                   .AddScoped<IInsecticideRepo, InsecticideRepo>()
                   .AddScoped<IFertilizerMixRepo, FertilizerMixRepo>()
                   .AddScoped<IFertilizerLandRepo, FertilizerLandRepo>()
                   .AddScoped<IInsecticideMixRepo, InsecticideMixRepo>()
                   .AddScoped<IInsecticideLandRepo, InsecticideLandRepo>()
                   .AddScoped(typeof(IFileRepo<>), typeof(FileRepo<>))
                   .AddScoped(typeof(IBaseRepo<>), typeof(BaseRepo<>));
}
