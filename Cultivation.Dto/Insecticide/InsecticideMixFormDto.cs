namespace Cultivation.Dto.Insecticide;

public class InsecticideMixFormDto
{
    public string Note { get; set; }
    public string Title { get; set; }

    public ICollection<InsecticideMixDto> Mixes { get; set; } = [];
}
