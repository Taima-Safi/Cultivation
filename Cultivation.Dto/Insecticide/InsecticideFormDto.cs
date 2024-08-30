using Cultivation.Shared.Enum;

namespace Cultivation.Dto.Insecticide;

public class InsecticideFormDto
{
    public string Title { get; set; }
    // public IFormFile File { get; set; }
    public string Description { get; set; }
    public string PublicTitle { get; set; }
    public InsecticideType Type { get; set; }
}
