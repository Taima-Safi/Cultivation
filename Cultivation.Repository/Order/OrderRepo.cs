using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Client;
using Cultivation.Dto.Order;
using Cultivation.Repository.Base;
using Cultivation.Repository.DataBase;
using Cultivation.Repository.Flower;
using FourthPro.Dto.Common;
using FourthPro.Shared.Exception;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cultivation.Repository.Order;

public class OrderRepo : IOrderRepo
{
    private readonly CultivationDbContext context;
    private readonly IFlowerRepo flowerRepo;
    private readonly IDbRepo dbRepo;
    private readonly IBaseRepo<OrderModel> baseRepo;

    public OrderRepo(CultivationDbContext context, IFlowerRepo flowerRepo, IDbRepo dbRepo, IBaseRepo<OrderModel> baseRepo)
    {
        this.context = context;
        this.flowerRepo = flowerRepo;
        this.dbRepo = dbRepo;
        this.baseRepo = baseRepo;
    }
    public async Task AddAsync(OrderFormDto dto)
    {
        //ToDo: check of remained count
        await dbRepo.BeginTransactionAsync();
        try
        {
            // var flowerModels = await flowerRepo.GetModelsByIdsAsync(dto.FlowerOrders.Select(f => f.FlowerId).ToList());
            var flowerStoreModel = await flowerRepo.GetFlowerStoreModelsByCodesAsync(dto.OrderDetails.Select(f => f.Code).ToList());

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
            List<OrderDetailModel> flowerOrderModels = new();
            List<Tuple<string, double>> failedFlowerLongs = new();
            List<Tuple<string, int, int>> failedFlowerCount = new();
            foreach (var flowerOrder in dto.OrderDetails)
            {
                if (!dicFlowerStoreModel.TryGetValue(flowerOrder.Code, out var possibleStores))
                    throw new NotFoundException($"Flower with Code {flowerOrder.Code} not found");

                var matchingStore = possibleStores.FirstOrDefault(f => f.FlowerLong == flowerOrder.Long);
                if (matchingStore == null)
                {
                    failedFlowerLongs.Add(new Tuple<string, double>(flowerOrder.Code, flowerOrder.Long));
                }
                else if (matchingStore.Count < flowerOrder.Count)
                {
                    failedFlowerCount.Add(new Tuple<string, int, int>(flowerOrder.Code, matchingStore.Count, flowerOrder.Count));
                }
                else
                {
                    if (dto.IsBought)
                        matchingStore.RemainedCount -= flowerOrder.Count;

                    matchingStore.Count -= flowerOrder.Count;

                    flowerOrderModels.Add(new OrderDetailModel
                    {
                        Count = flowerOrder.Count,
                        OrderId = orderModel.Entity.Id,
                        FlowerStoreId = matchingStore.Id,
                    });
                }
            }
            var errorMessages = new List<string>();
            if (failedFlowerLongs.Count != 0)
            {
                var x = string.Join(", ", failedFlowerLongs.Select(c => $"Code: {c.Item1}, Long: {c.Item2} "));
                errorMessages.Add($"Aşağıdaki kodlu ve nolu çiçekler depoda yoktur\n {x}");
            }
            if (failedFlowerCount.Count != 0)
            {
                var x = string.Join(", ", failedFlowerCount.Select(c => $"Code: {c.Item1}, Stored Count: {c.Item2}, Ordered Count: {c.Item3} "));
                errorMessages.Add($"Insufficient count for flower with Code and Long\n {x}");
            }
            if (errorMessages.Count > 0)
                throw new NotFoundException(string.Join(" | ", errorMessages));

            await context.OrderDetail.AddRangeAsync(flowerOrderModels);

            await dbRepo.SaveChangesAsync();
            await dbRepo.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await dbRepo.RollbackTransactionAsync();
            throw;
        }
    }
    public async Task<CommonResponseDto<List<OrderDto>>> GetAllAsync(bool isBought, DateTime? from, DateTime? to, int pageSize, int pageNum)
    {
        Expression<Func<OrderModel, bool>> expression = o =>
        isBought ? ((!from.HasValue || o.BoughtDate >= from) && (!to.HasValue || o.BoughtDate >= to))
        : ((!from.HasValue || o.OrderDate >= from) && (!to.HasValue || o.OrderDate >= to))
        && o.IsValid;

        var x = await context.Order.Where(expression)
            .Skip(pageNum * pageSize)
            .Take(pageSize).Select(o => new OrderDto
            {
                Id = o.Id,
                IsBought = isBought,
                Number = o.Number,
                OrderDate = o.OrderDate,
                BoughtDate = o.BoughtDate,
                Client = new ClientDto
                {
                    Id = o.Client.Id,
                    Name = o.Client.Name,
                    IsLocal = o.Client.IsLocal
                },
                OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
                {
                    Count = od.Count,
                    Code = od.FlowerStore.Code,
                    Long = od.FlowerStore.FlowerLong
                }).ToList()
            }).ToListAsync();

        bool hasNextPage = false;
        if (x.Count > 0)
            hasNextPage = await baseRepo.CheckIfHasNextPageAsync(expression, pageSize, pageNum);

        return new CommonResponseDto<List<OrderDto>>(x, hasNextPage);
    }
    public async Task UpdateOrderStatusAsync(long orderId, DateTime boughtDate)
    {
        if (!await CheckIfExistAsync(orderId))
            throw new NotFoundException("order not found..");

        await context.Order.Where(o => o.Id == orderId && o.IsValid).ExecuteUpdateAsync(o => o.SetProperty(o => o.IsBought, true)
       .SetProperty(o => o.BoughtDate, boughtDate));
    }
    public async Task UpdateAsync(UpdateOrderDto dto)
    {
        if (!await CheckIfExistAsync(dto.Id))
            throw new NotFoundException("order not found..");

        await context.Order.Where(o => o.Id == dto.Id && o.IsValid).ExecuteUpdateAsync(o => o.SetProperty(o => o.ClientId, dto.ClientId)
       .SetProperty(o => o.OrderDate, dto.OrderDate));
    }
    //Update orderDetails
    public async Task RemoveAsync(long id)
    {
        if (!await CheckIfExistAsync(id))
            throw new NotFoundException("Order not found..");

        await context.Order.Where(o => o.Id == id && o.IsValid).ExecuteUpdateAsync(o => o.SetProperty(o => o.IsValid, false));
    }

    public async Task<bool> CheckIfExistAsync(long id)
 => await context.Order.Where(c => c.Id == id && c.IsValid).AnyAsync();
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
