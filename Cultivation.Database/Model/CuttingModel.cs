namespace Cultivation.Database.Model;

public class CuttingModel : BaseModel
{
    public string Title { get; set; }
    public string Type { get; set; }// flower type
    public int Age { get; set; } // in days
    public ICollection<CuttingColorModel> CuttingColors { get; set; }
}