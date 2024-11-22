namespace Cultivation.Dto.Flower;

public class AddFlowerFormDto
{
    public DateTime Date { get; set; }
    public string Worker { get; set; }

    public List<FlowerFormDto> Flowers { get; set; }
}
