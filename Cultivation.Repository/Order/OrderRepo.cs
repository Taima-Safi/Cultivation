using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Order;
using Cultivation.Repository.DataBase;
using Cultivation.Repository.Flower;
using FourthPro.Shared.Exception;
using Microsoft.EntityFrameworkCore;

namespace Cultivation.Repository.Order;

public class OrderRepo : IOrderRepo
{
    private readonly CultivationDbContext context;
    private readonly IFlowerRepo flowerRepo;
    private readonly IDbRepo dbRepo;

    public OrderRepo(CultivationDbContext context, IFlowerRepo flowerRepo, IDbRepo dbRepo)
    {
        this.context = context;
        this.flowerRepo = flowerRepo;
        this.dbRepo = dbRepo;
    }
    public async Task AddAsync(OrderFormDto dto)
    {
        //ToDo: check of remained count
        await dbRepo.BeginTransactionAsync();
        try
        {
            // var flowerModels = await flowerRepo.GetModelsByIdsAsync(dto.FlowerOrders.Select(f => f.FlowerId).ToList());
            var flowerStoreModel = await flowerRepo.GetFlowerStoreModelsByCodesAsync(dto.FlowerOrders.Select(f => f.Code).ToList());

            var dicFlowerStoreModel = flowerStoreModel.GroupBy(f => f.Code).ToDictionary(x => x.Key, x => x.ToList());

            //  var failingOrders = dto.FlowerOrders.Where(fo => !flowerStoreModel.Any(f => f.Code == fo.Code && f.FlowerLong == fo.Long)).Select(x => x.Code).ToList();
            //  if (failingOrders.Any())
            //      throw new NotFoundException($"The following flowers do not have sufficient remaining count: {string.Join(", ", failingOrders)}");

            var order = new OrderModel
            {
                ClientId = dto.ClientId,
                IsBought = dto.IsBought,
                OrderDate = dto.OrderDate,
                BoughtDate = dto.BoughtDate,
            };
            var orderModel = await context.Order.AddAsync(order);
            await context.SaveChangesAsync();

            dto.Number = GetBillNumber(order.Id);
            order.Number = dto.Number;

            List<FlowerOrderModel> flowerOrderModels = [];

            //foreach(var x in flowerStoreModel)
            //{
            //    foreach (var flower in dto.FlowerOrders)
            //    {
            //        if(x.Code == flower.Code && x.FlowerLong == flower.Long)
            //            if (x.Count < flower.Count)
            //                throw new NotFoundException("Count not found");

            //        flowerOrderModels.Add(new FlowerOrderModel
            //        {
            //            Count = flower.Count,
            //            FlowerStoreId = x.Id,
            //            OrderId = orderModel.Entity.Id,
            //        });
            //    }
            //}
            List<Tuple<string, double>> faildFlowerLongs = [];
            foreach (var flowerOrder in dto.FlowerOrders)
            {
                if (!dicFlowerStoreModel.TryGetValue(flowerOrder.Code, out var possibleStores))
                    throw new NotFoundException($"Flower with Code {flowerOrder.Code} not found");

                var matchingStore = possibleStores.FirstOrDefault(f => f.FlowerLong == flowerOrder.Long);
                if (matchingStore == null)
                {
                    throw new NotFoundException($"Flower with Code {flowerOrder.Code} and Long {flowerOrder.Long} not found in the store.");
                }
                else
                {
                    if (matchingStore.Count < flowerOrder.Count)
                        throw new NotFoundException($"Insufficient count for flower with Code {flowerOrder.Code} and Long {flowerOrder.Long}.");

                    if (dto.IsBought)
                        matchingStore.RemainedCount -= flowerOrder.Count;

                    matchingStore.Count -= flowerOrder.Count;

                    flowerOrderModels.Add(new FlowerOrderModel
                    {
                        Count = flowerOrder.Count,
                        OrderId = orderModel.Entity.Id,
                        FlowerStoreId = matchingStore.Id,
                    });
                }

            }

            await context.FlowerOrder.AddRangeAsync(flowerOrderModels);

            await dbRepo.SaveChangesAsync();
            await dbRepo.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await dbRepo.RollbackTransactionAsync();
            throw;
        }
    }
    public async Task UpdateOrderStatusAsync(long orderId, DateTime boughtDate)
       => await context.Order.Where(o => o.Id == orderId && o.IsValid).ExecuteUpdateAsync(o => o.SetProperty(o => o.IsBought, true)
       .SetProperty(o => o.BoughtDate, boughtDate));

    public async Task RemoveAsync(long id)
       => await context.Order.Where(o => o.Id == id && o.IsValid).ExecuteUpdateAsync(o => o.SetProperty(o => o.IsValid, false));

    public string GetBillNumber(long id)
    {
        string newString = id.ToString();
        var random = new Random();
        for (int i = 0; i < 4; i++)
        {
            int randomNumber = random.Next(10);
            string stringNumber = randomNumber.ToString();
            newString += stringNumber;
        }
        return newString;
    }
}
