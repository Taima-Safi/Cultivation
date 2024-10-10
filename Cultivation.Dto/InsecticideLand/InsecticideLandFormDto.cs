namespace Cultivation.Dto.InsecticideLand;

public class InsecticideLandFormDto
{
    public string Note { get; set; }
    public DateTime Date { get; set; }

    public List<long> LandIds { get; set; } = [];
    public List<InsecticideMixDto> Mixes { get; set; } = [];



    //public IFormFile File { get; set; }
    //public double Liter { get; set; }
    //public double? Quantity { get; set; } // if powder -> for one liter
    //public long InsecticideId { get; set; }
}
