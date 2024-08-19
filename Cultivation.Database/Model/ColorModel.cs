namespace Cultivation.Database.Model;

public class ColorModel : BaseModel
{
    public string Code { get; set; }
    public string Title { get; set; }
    public ICollection<CuttingColorModel> CuttingColors { get; set; }

}
