﻿namespace Cultivation.Database.Model;

public class FlowerModel : BaseModel
{
    public int Count { get; set; }
    public string Note { get; set; }
    public double Long { get; set; }
    public DateTime Date { get; set; }
    public string Worker { get; set; }

    public long CuttingLandId { get; set; }
    public CuttingLandModel CuttingLand { get; set; }
}
