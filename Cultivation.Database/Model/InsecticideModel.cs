using Cultivation.Shared.Enum;

namespace Cultivation.Database.Model;

public class InsecticideModel : BaseModel
{
    //public string File { get; set; }
    public string Title { get; set; }
    public string PublicTitle { get; set; }
    public string Description { get; set; }
    public InsecticideType Type { get; set; }
    public ICollection<InsecticideLandModel> InsecticideLands { get; set; }
}
