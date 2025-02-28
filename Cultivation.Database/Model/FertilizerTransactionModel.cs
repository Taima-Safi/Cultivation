namespace Cultivation.Database.Model;

public class FertilizerTransactionModel : BaseModel
{
    public bool IsAdd { get; set; }
    public DateTime Date { get; set; }
    public double QuantityChange { get; set; }

    public long FertilizerId { get; set; }
    public FertilizerModel Fertilizer { get; set; }
}
