﻿namespace Cultivation.Database.Model;

public class InsecticideApplicableMixModel : BaseModel
{
    public double DonumCount { get; set; }
    public double CurrentDonumCount { get; set; }

    public long InsecticideMixId { get; set; }
    public InsecticideMixModel InsecticideMix { get; set; }
}
