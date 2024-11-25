﻿namespace Cultivation.Database.Model;

public class OrderModel : BaseModel
{
    public string Number { get; set; }
    public bool IsBought { get; set; }

    public long ClientId { get; set; }
    public ClientModel Client { get; set; }
    public ICollection<FlowerOrderModel> FlowerOrders { get; set; }
}