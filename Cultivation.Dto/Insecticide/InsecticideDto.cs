using Cultivation.Shared.Enum;

namespace Cultivation.Dto.Insecticide;

public class InsecticideDto
{
    public long Id { get; set; }
    //public string File { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string PublicTitle { get; set; }
    public InsecticideType Type { get; set; }
}
