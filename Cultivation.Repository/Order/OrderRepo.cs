using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Order;
using Cultivation.Repository.DataBase;
using Cultivation.Repository.Flower;
using FourthPro.Shared.Exception;

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
                IsBought = dto.IsBought
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
            foreach (var flower in dto.FlowerOrders)
            {
                if (!dicFlowerStoreModel.TryGetValue(flower.Code, out var possibleStores))
                    throw new NotFoundException($"Flower with Code {flower.Code} not found");

                var matchingStore = possibleStores.FirstOrDefault(f => f.FlowerLong == flower.Long);
                if (matchingStore == null)
                    throw new NotFoundException($"Flower with Code {flower.Code} and Long {flower.Long} not found in the store.");

                if (matchingStore.RemainedCount < flower.Count)
                    throw new NotFoundException($"Insufficient count for flower with Code {flower.Code} and Long {flower.Long}.");

                flowerOrderModels.Add(new FlowerOrderModel
                {
                    Count = flower.Count,
                    OrderId = orderModel.Entity.Id,
                    FlowerStoreId = matchingStore.Id,
                });
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
