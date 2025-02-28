namespace Cultivation.Dto.Fertilizer;
public class FertilizerTransactionDto
{
    public long Id { get; set; }
    public bool IsAdd { get; set; }
    public DateTime Date { get; set; }
    public double QuantityChange { get; set; }

    public FertilizerDto Fertilizer { get; set; }
}
