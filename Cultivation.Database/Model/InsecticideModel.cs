using Cultivation.Shared.Enum;

namespace Cultivation.Database.Model;

public class InsecticideModel : BaseModel
{
    public string Note { get; set; }
    public string File { get; set; }
    public string Title { get; set; }
    public string PublicTitle { get; set; }
    public InsecticideType Type { get; set; }
}
