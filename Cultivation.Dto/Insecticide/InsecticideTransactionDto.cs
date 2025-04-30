namespace Cultivation.Dto.Insecticide;

public class InsecticideTransactionDto
{
    public long Id { get; set; }
    public bool IsAdd { get; set; }
    public DateTime Date { get; set; }
    public double QuantityChange { get; set; }

    public InsecticideDto Insecticide { get; set; }
}