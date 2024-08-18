using Cultivation.Database.Context;

namespace Cultivation.Repository.FertilizerLand;

public class FertilizerLandRepo : IFertilizerLandRepo
{
    private readonly CultivationDbContext context;

    public FertilizerLandRepo(CultivationDbContext context)
    {
        this.context = context;
    }

}
