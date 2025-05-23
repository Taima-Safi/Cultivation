﻿using Cultivation.Dto.Flower;

namespace Cultivation.Dto.Order;

public class OrderDetailDto
{
    public long Id { get; set; }
    public int Count { get; set; }

    public OrderDto Order { get; set; }
    public FlowerStoreDto FlowerStore { get; set; }
}
