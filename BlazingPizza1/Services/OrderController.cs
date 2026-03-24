using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazingPizza.Data;

namespace BlazingPizza;

[Route("orders")]
[ApiController]
public class OrdersController : Controller
{
    private readonly PizzaStoreContext _db;

    public OrdersController(PizzaStoreContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderWithStatus>>> GetOrders()
    {
        var orders = await _db.Orders
        .Include(o => o.Pizzas).ThenInclude(p => p.Special)
        .Include(o => o.Pizzas).ThenInclude(p => p.Toppings).ThenInclude(t => t.Topping)
        .OrderByDescending(o => o.CreatedTime)
        .ToListAsync();

        return orders.Select(o => OrderWithStatus.FromOrder(o)).ToList();
    }

    [HttpPost]
    public async Task<ActionResult<int>> PlaceOrder(Order order)
    {
        Console.WriteLine("=== Placing a new order ===");
        Console.WriteLine($"Number of pizzas in order: {order.Pizzas?.Count ?? 0}");

        if (order.Pizzas == null || order.Pizzas.Count == 0)
        {
            Console.WriteLine("Warning: Order has no pizzas!");
            return BadRequest("Order must contain at least one pizza.");
        }

        order.CreatedTime = DateTime.Now;

        foreach (var pizza in order.Pizzas)
        {
            if (pizza.Special == null)
            {
                Console.WriteLine("Error: Pizza.Special is null!");
            }
            else
            {
                pizza.SpecialId = pizza.Special.Id;
                Console.WriteLine($"Pizza special: {pizza.Special.Name} (Id: {pizza.SpecialId})");
                pizza.Special = null;
            }

            if (pizza.Toppings != null)
            {
                foreach (var topping in pizza.Toppings)
                {
                    if (topping.Topping == null)
                    {
                        Console.WriteLine("Warning: Topping.Topping is null!");
                    }
                    else
                    {
                        Console.WriteLine($"Topping: {topping.Topping.Name} (Id: {topping.ToppingId})");
                    }
                }
            }
        }

        Console.WriteLine("Attaching order to DB...");
        _db.Orders.Attach(order);

        try
        {
            await _db.SaveChangesAsync();
            Console.WriteLine($"Order saved successfully! OrderId: {order.OrderId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving order: {ex.Message}");
            return StatusCode(500, "Failed to save order");
        }

        Console.WriteLine("=== Order processing complete ===");
        return order.OrderId;
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderWithStatus>> GetOrderWithStatus(int orderId)
    {
        var order = await _db.Orders
            .Where(o => o.OrderId == orderId)
            .Include(o => o.Pizzas).ThenInclude(p => p.Special)
            .Include(o => o.Pizzas).ThenInclude(p => p.Toppings).ThenInclude(t => t.Topping)
            .SingleOrDefaultAsync();

        if (order == null)
        {
            return NotFound();
        }

        return OrderWithStatus.FromOrder(order);
    }





}