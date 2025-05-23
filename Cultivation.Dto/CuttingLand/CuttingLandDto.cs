﻿using Cultivation.Dto.Cutting;
using Cultivation.Dto.InsecticideLand;
using Cultivation.Dto.Land;

namespace Cultivation.Dto.CuttingLand;

public class CuttingLandDto
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public long Quantity { get; set; }
    public bool IsActive { get; set; }

    public LandDto Land { get; set; }
    public CuttingColorDto CuttingColor { get; set; }
    //  public List<FertilizerMixLandDto> FertilizerMixLands { get; set; } = [];
    public List<InsecticideMixLandDto> InsecticideMixLands { get; set; } = [];
}
