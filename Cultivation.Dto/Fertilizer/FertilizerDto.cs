﻿using Cultivation.Shared.Enum;

namespace Cultivation.Dto.Fertilizer;

public class FertilizerDto
{
    public long Id { get; set; }
    public string NPK { get; set; }
    public string Title { get; set; }
    public string PublicTitle { get; set; }
    //public double Price { get; set; }//kg
    public string File { get; set; }
    public string Description { get; set; }
    public FertilizerType Type { get; set; }
}
