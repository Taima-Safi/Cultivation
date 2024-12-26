namespace Cultivation.Dto.Insecticide;

public class GetInsecticideMixDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Note { get; set; }

    public List<InsecticideMixDetailDto> MixDetails { get; set; } = [];
}
