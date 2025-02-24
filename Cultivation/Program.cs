using Cultivation.Configuration;
using Cultivation.Database.Context;
using Cultivation.Middleware;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ErrorHandlerMiddleware>();
builder.Services.AddTransient<AuthenticationMiddleware>();
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

builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.ConfigureRepos();

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

app.UseSwaggerUI();

app.UseAuthorization();
app.UseAuthentication();

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();

app.MapControllers();

app.Run();