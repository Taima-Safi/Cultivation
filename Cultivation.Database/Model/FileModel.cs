namespace Cultivation.Database.Model;

public class FileModel : BaseModel
{
    public int Id { get; set; }
    public byte[] Data { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
}
