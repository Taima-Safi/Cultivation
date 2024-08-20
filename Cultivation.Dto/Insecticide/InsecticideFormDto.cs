using Cultivation.Shared.Enum;
using Microsoft.AspNetCore.Http;

namespace Cultivation.Dto.Insecticide;

public class InsecticideFormDto
{
    public string Note { get; set; }
    public IFormFile File { get; set; }
    public string Title { get; set; }
    public string PublicTitle { get; set; }
    public InsecticideType Type { get; set; }
}
