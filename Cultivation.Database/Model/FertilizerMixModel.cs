using Cultivation.Shared.Enum;

namespace Cultivation.Database.Model;

public class FertilizerMixModel : BaseModel
{
    public string Title { get; set; }
    public FertilizerType Type { get; set; }

    public ICollection<FertilizerMixLandModel> FertilizerMixLands { get; set; }
    public ICollection<FertilizerMixDetailModel> FertilizerMixDetails { get; set; }

}
