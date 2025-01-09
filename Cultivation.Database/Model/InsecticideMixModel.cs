using Cultivation.Shared.Enum;

namespace Cultivation.Database.Model;

public class InsecticideMixModel : BaseModel
{
    public string Note { get; set; }
    public string Title { get; set; }
    public ColorType Color { get; set; }

    public ICollection<InsecticideMixLandModel> InsecticideMixLand { get; set; }
    public ICollection<InsecticideMixDetailModel> InsecticideMixDetails { get; set; }

}
