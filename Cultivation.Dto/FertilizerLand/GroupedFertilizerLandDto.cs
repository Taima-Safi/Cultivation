namespace Cultivation.Dto.FertilizerLand;

public class GroupedFertilizerLandDto
{
    public DateTime Date { get; set; }
    public List<FertilizerLandDto> FertilizerLandGroup { get; set; }
}
