using System.ComponentModel.DataAnnotations;

namespace Cultivation.Database.Model;

public class BaseModel
{
    [Key]
    public long Id { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsValid { get; set; } = true;
}
