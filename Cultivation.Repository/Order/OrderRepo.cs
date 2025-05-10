using Cultivation.Database.Context;
using Cultivation.Database.Model;
using Cultivation.Dto.Client;
using Cultivation.Dto.Common;
using Cultivation.Dto.Flower;
using Cultivation.Dto.Order;
using Cultivation.Repository.Base;
using Cultivation.Repository.DataBase;
using Cultivation.Repository.Flower;
using Cultivation.Shared.Exception;
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
            var flowerStoreModels = await flowerRepo.GetFlowerStoreModelsByCodesAsync(dto.FlowerOrderDetails.Select(f => f.Code).ToList());

            var dicFlowerStoreModel = flowerStoreModels.GroupBy(f => f.Code).ToDictionary(x => x.Key, x => x.ToList());

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

            List<OrderDetailModel> flowerOrderModels = new();
            List<Tuple<string, double>> failedFlowerLongs = new();
            List<Tuple<string, int, int>> failedFlowerCount = new();
            foreach (var flowerOrder in dto.FlowerOrderDetails)
            {
                if (!dicFlowerStoreModel.TryGetValue(flowerOrder.Code, out var possibleStores))
                    throw new NotFoundException($"Flower with Code {flowerOrder.Code} not found"); //TODO :should choice if he want <Continue>

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
    public async Task<int> GetOrderCountAsync()
    => await context.Order.Where(o => !o.IsBought && o.IsValid).CountAsync();

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
                    Id = od.Id,
                    Count = od.Count,
                    FlowerStore = new FlowerStoreDto
                    {
                        Id = od.FlowerStore.Id,
                        Code = od.FlowerStore.Code,
                        FlowerLong = od.FlowerStore.FlowerLong,
                    }
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

        var orderModel = await GetModelByIdAsync(orderId);
        var storeModels = await flowerRepo.GetStoreModelsByIdsAsync(orderModel.OrderDetails.Select(od => od.FlowerStoreId).ToList());

        var storeAvailability = storeModels.ToDictionary(store => store.Code, store => store.RemainedCount /*+ store.ExternalCount*/);
        var missingFlowers = new List<(string StoreCode, int MissingCount)>();
        foreach (var detail in orderModel.OrderDetails)
        {
            if (storeAvailability.TryGetValue(detail.FlowerStore.Code, out var availableCount))
            {
                if (availableCount < detail.Count)
                {
                    int missingCount = detail.Count - availableCount;
                    missingFlowers.Add((detail.FlowerStore.Code, missingCount));
                }
            }
        }
        if (missingFlowers.Any())
        {
            var missingDetails = missingFlowers
                .Select(m => $"Store {m.StoreCode}: {m.MissingCount} flowers not found")
                .ToList();

            throw new NotFoundException(string.Join(", ", missingDetails));
        }
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
        await dbRepo.BeginTransactionAsync();
        try
        {
            if (!await CheckIfExistAsync(id))
                throw new NotFoundException("Order not found..");
            var orderModel = await context.Order.Include(o => o.OrderDetails).ThenInclude(od => od.Order)
                .Include(o => o.OrderDetails).ThenInclude(od => od.FlowerStore).Where(o => o.Id == id && o.IsValid).FirstOrDefaultAsync();

            if (!orderModel.IsBought)
                foreach (var detail in orderModel.OrderDetails)
                    detail.FlowerStore.Count += detail.Count;

            await context.Order.Where(o => o.Id == id && o.IsValid).ExecuteUpdateAsync(o => o.SetProperty(o => o.IsValid, false));

            await dbRepo.SaveChangesAsync();
            await dbRepo.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await dbRepo.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<OrderModel> GetModelByIdAsync(long id)
     => await context.Order.Where(o => o.Id == id && o.IsValid).Include(o => o.OrderDetails).FirstOrDefaultAsync();

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
