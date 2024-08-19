namespace Cultivation.Database.Model;

public class FertilizerModel : BaseModel
{
    public string NPK { get; set; }
    public string Title { get; set; }
    public string PublicTitle { get; set; }
    //public double Price { get; set; }//kg
    public string File { get; set; }
    public string Description { get; set; }
    public ICollection<FertilizerLandModel> FertilizerLands { get; set; }
}
