using Cultivation.Database.Context;
using Cultivation.Repository.Base;
using Cultivation.Repository.Color;
using Cultivation.Repository.Cutting;
using Cultivation.Repository.CuttingLand;
using Cultivation.Repository.Fertilizer;
using Cultivation.Repository.FertilizerLand;
using Cultivation.Repository.File;
using Cultivation.Repository.Flower;
using Cultivation.Repository.Insecticide;
using Cultivation.Repository.InsecticideLand;
using Cultivation.Repository.Land;
using FourthPro.Middleware;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ErrorHandlerMiddleware>();
//builder.Services.AddTransient<AuthMiddleware>();
builder.Services.AddHttpContextAccessor();

#region Database
var connectionString = builder.Configuration.GetConnectionString(builder.Environment.IsProduction() ? "StagingMonsterServer" : "StagingMonsterServer");
builder.Services.AddDbContext<CultivationDbContext>(options =>
{
    options.UseSqlServer(connectionString/*, ServerVersion.AutoDetect(connectionString)*/);
});
#endregion

#region Caching Configuration
builder.Services.AddMemoryCache(opt =>
{
    opt.TrackLinkedCacheEntries = true;
}).AddResponseCaching();
#endregion

//builder.Services.ConfigureAuthentication(builder.Configuration);
//builder.Services.ConfigureSwagger();
//builder.Services.ConfigureRepos();
//builder.Services.ConfigureServices();
builder.Services.AddScoped<ILandRepo, LandRepo>();
builder.Services.AddScoped<IColorRepo, ColorRepo>();
builder.Services.AddScoped<ICuttingRepo, CuttingRepo>();
builder.Services.AddScoped<ICuttingLandRepo, CuttingLandRepo>();
builder.Services.AddScoped<IFertilizerRepo, FertilizerRepo>();
builder.Services.AddScoped<IFertilizerLandRepo, FertilizerLandRepo>();
builder.Services.AddScoped<IInsecticideRepo, InsecticideRepo>();
builder.Services.AddScoped<IInsecticideLandRepo, InsecticideLandRepo>();
builder.Services.AddScoped<IFlowerRepo, FlowerRepo>();
builder.Services.AddScoped(typeof(IFileRepo<>), typeof(FileRepo<>));
builder.Services.AddScoped(typeof(IBaseRepo<>), typeof(BaseRepo<>));

var app = builder.Build();
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseCors(cors => cors
.AllowAnyMethod()
.AllowAnyHeader()
.SetIsOriginAllowed(origin => true)
.AllowCredentials());

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSwagger();

app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.DocExpansion(DocExpansion.None);
    s.DisplayRequestDuration();
    s.EnableTryItOutByDefault();
});
app.UseRouting();

app.UseMiddleware<ErrorHandlerMiddleware>();
//app.UseMiddleware<AuthMiddleware>();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();