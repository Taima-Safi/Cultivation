namespace Cultivation.Database.Model;

public class CuttingColorModel : BaseModel
{
    public string Code { get; set; }// flower color + type

    public long CuttingId { get; set; }
    public CuttingModel Cutting { get; set; }
    public long ColorId { get; set; }
    public ColorModel Color { get; set; }
}
