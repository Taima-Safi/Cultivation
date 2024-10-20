﻿namespace Cultivation.Database.Model;

public class CuttingLandModel : BaseModel
{
    public DateTime Date { get; set; }
    public long Quantity { get; set; }
    public bool IsActive { get; set; }

    public long LandId { get; set; }
    public LandModel Land { get; set; }

    public long CuttingColorId { get; set; }
    public CuttingColorModel CuttingColor { get; set; }
}
