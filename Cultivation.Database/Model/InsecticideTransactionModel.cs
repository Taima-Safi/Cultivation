namespace Cultivation.Database.Model;

public class InsecticideTransactionModel : BaseModel
{
    public bool IsAdd { get; set; }
    public DateTime Date { get; set; }
    public double QuantityChange { get; set; }

    public long InsecticideId { get; set; }
    public InsecticideModel Insecticide { get; set; }
}
