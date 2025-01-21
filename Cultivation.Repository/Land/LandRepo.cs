using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.CuttingLand;
using Cultivation.Dto.Fertilizer;
using Cultivation.Dto.FertilizerLand;
using Cultivation.Dto.Insecticide;
using Cultivation.Dto.InsecticideLand;
using Cultivation.Dto.Land;
using FourthPro.Shared.Exception;
using Microsoft.EntityFrameworkCore;

namespace Cultivation.Repository.Land;

public class LandRepo : ILandRepo
{
    private readonly CultivationDbContext context;

    public LandRepo(CultivationDbContext context)
    {
        this.context = context;
    }
    public async Task<long> AddAsync(LandFormDto dto)
    {
        if (dto.ParentId.HasValue)
        {
            var parent = await GetLandModelAsync(dto.ParentId.Value);

            if (parent == null)
                throw new NotFoundException("Parent land Not Found..");

            var childrenLandsSize = parent.Children.Select(l => l.Size).Sum();
            if (parent.Size < (childrenLandsSize + dto.Size))
                throw new NotFoundException("No land  size used completely");
        }
        var land = await context.Land.AddAsync(new LandModel
        {
            Size = dto.Size,
            Title = dto.Title,
            Location = dto.Location,
            ParentId = dto.ParentId
        });
        await context.SaveChangesAsync();
        return land.Entity.Id;
    }
    public async Task<List<LandDto>> GetAllAsync(string title, string mixTitle, DateTime? mixedDate, bool? isFertilizer,
        double? size, bool justChildren, bool isNoneActive, bool forMix)
    {
        var landModels = await context.Land.Include(l => l.CuttingLands)
            .Include(cl => cl.FertilizerMixLands).ThenInclude(fml => fml.FertilizerMix)
            .Include(cl => cl.InsecticideMixLands).ThenInclude(iml => iml.InsecticideMix)
            .Where(l => (string.IsNullOrEmpty(title) || l.Title.Contains(title))
        && (!size.HasValue || l.Size == size)
        && l.IsValid)
            .Select(l => new LandDto
            {
                Id = l.Id,
                Size = l.Size,
                Title = l.Title,
                Location = l.Location,
                ParentId = l.Parent.Id,
                Children = l.Children.Select(l => new LandDto
                {
                    Id = l.Id,
                    Size = l.Size,
                    Title = l.Title,
                    Location = l.Location,
                    ParentId = l.Parent.Id,
                }).ToList(),
                FertilizerMixLands = l.FertilizerMixLands.Where(fml => (!isFertilizer.HasValue || (string.IsNullOrEmpty(mixTitle) || fml.FertilizerMix.Title.Contains(mixTitle))
                && (!mixedDate.HasValue || fml.Date == mixedDate))
                && fml.IsValid).Select(m => new FertilizerMixLandDto
                {
                    Id = m.Id,
                    Date = m.Date,
                    FertilizerMix = new GetFertilizerMixDto
                    {
                        Id = m.FertilizerMix.Id,
                        Type = m.FertilizerMix.Type,
                        Color = m.FertilizerMix.Color,
                        Title = m.FertilizerMix.Title,
                    }
                }).ToList(),
                InsecticideMixLands = l.InsecticideMixLands.Where(iml => (isFertilizer.HasValue || (string.IsNullOrEmpty(mixTitle) || iml.InsecticideMix.Title.Contains(mixTitle))
                && (!mixedDate.HasValue || iml.Date == mixedDate)) && iml.IsValid).Select(i => new InsecticideMixLandDto
                {
                    Id = i.Id,
                    Date = i.Date,
                    InsecticideMix = new GetInsecticideMixDto
                    {
                        Id = i.InsecticideMix.Id,
                        Note = i.InsecticideMix.Note,
                        Title = i.InsecticideMix.Title,
                        Color = i.InsecticideMix.Color,
                    }
                }).ToList(),
                CuttingLands = l.CuttingLands.Select(l => new CuttingLandDto
                {
                    Id = l.Id,
                    IsActive = l.IsActive,
                }).ToList()
            }).ToListAsync();
        var parents = landModels.Where(l => !l.ParentId.HasValue).ToList();
        var result = new List<LandDto>();
        List<LandDto> resultWithoutChildren = [];
        List<LandDto> grandWithChild = [];
        LandDto grand = new();

        foreach (var parent in parents)
        {
            result.Add(GetChildrenAsync(parent, grand, landModels, resultWithoutChildren, grandWithChild));
        }
        //grandWithChild.AddRange(parents.Where(x => x.CuttingLands.Any(x => x.IsActive)));
        //grandWithChild.AddRange(resultWithoutChildren.Where(x => grandWithChild.Any(v => v.Id != x.Id) && x.CuttingLands.Any(x => x.IsActive)));
        if (forMix) // to take grandFather && land has no children
            return grandWithChild;
        //return landModels.Where(l => l.ParentId == null || !l.Children.Any()).ToList();

        if (justChildren)
            return resultWithoutChildren.Where(l => !isNoneActive || (l.CuttingLands.All(cl => !cl.IsActive) || !l.CuttingLands.Any())).ToList();

        return result;
    }
    public async Task<LandDto> GetByIdAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Land not found..");

        var landModels = await context.Land.Where(l => l.IsValid)
            .Select(l => new LandDto
            {
                Id = l.Id,
                Size = l.Size,
                Title = l.Title,
                Location = l.Location,
                ParentId = l.Parent.Id,
                Children = l.Children.Select(l => new LandDto
                {
                    Id = l.Id,
                    Size = l.Size,
                    Title = l.Title,
                    Location = l.Location,
                    ParentId = l.Parent.Id,
                }).ToList(),

            }).ToListAsync();

        var parent = landModels.Where(l => l.Id == id).FirstOrDefault();

        var result = GetChildrenAsync(parent, new(), landModels, new(), new());
        return result;
    }
    private LandDto GetChildrenAsync(LandDto parent, LandDto grand, List<LandDto> allLands, List<LandDto> resultWithoutChildren, List<LandDto> grandWithChild)
    {
        var children = allLands.Where(a => a.ParentId == parent.Id).ToList();

        if (parent.ParentId == null)
        {
            //grand = parent;
            grand = new LandDto
            {
                Id = parent.Id,
                Location = parent.Location,
                ParentId = parent.ParentId,
                Size = parent.Size,
                Title = parent.Title,
                Children = [],
                FertilizerMixLands = parent.FertilizerMixLands.ToList(),
                InsecticideMixLands = parent.InsecticideMixLands.ToList(),
                CuttingLands = parent.CuttingLands.Where(x => x.IsActive).ToList(),
            };
            //grand.Children.Clear();
            grandWithChild.Add(grand);
        }

        if (children.Count == 0)
            resultWithoutChildren.Add(parent);

        parent.Children = children;

        foreach (var child in children)
        {
            if (child.Children.Count == 0 && child.CuttingLands.Any(x => x.IsActive))
            {
                var grandx = grandWithChild.LastOrDefault(x => x.ParentId == null);
                grandx.Children.Add(child);
            }
            GetChildrenAsync(child, grand, allLands, resultWithoutChildren, grandWithChild);
        }

        return parent;
    }
    public async Task UpdateAsync(long id, LandFormDto dto)
        => await context.Land.Where(l => l.Id == id && l.IsValid).ExecuteUpdateAsync(l => l.SetProperty(l => l.Title, dto.Title).SetProperty(l => l.Size, dto.Size)
        .SetProperty(l => l.Location, dto.Location));
    public async Task RemoveAsync(long id)
        => await context.Land.Where(l => l.Id == id && l.IsValid).ExecuteUpdateAsync(l => l.SetProperty(l => l.IsValid, false));
    public async Task RemoveAllAsync()
        => await context.Land.Where(l => l.IsValid).ExecuteUpdateAsync(l => l.SetProperty(l => l.IsValid, false));

    public async Task<LandModel> GetLandModelAsync(long id)
        => await context.Land.Where(l => l.Id == id && l.IsValid).Include(l => l.Children).FirstOrDefaultAsync();

    public async Task<bool> CheckIfExistAsync(long id)
        => await context.Land.Where(l => l.Id == id && l.IsValid).AnyAsync();

    public async Task<bool> CheckIfExistByIdsAsync(List<long> ids)
        => await context.Land.AnyAsync(l => ids.Contains(l.Id) && l.IsValid);
}
