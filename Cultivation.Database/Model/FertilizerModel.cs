using Cultivation.Shared.Enum;

namespace Cultivation.Database.Model;

public class FertilizerModel : BaseModel
{
    public string Title { get; set; }
    public double Price { get; set; }//kg
    public FertilizerType Type { get; set; }
}
