using Microsoft.AspNetCore.Http;

namespace Cultivation.Dto.InsecticideLand;

public class InsecticideLandFormDto
{
    public IFormFile File { get; set; }
    public string Note { get; set; }
    public double Liter { get; set; }
    public DateTime Date { get; set; }
    public double? Quantity { get; set; } // for one liter

    public long LandId { get; set; }
    public long InsecticideId { get; set; }
}
