using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelFoodCms.Data;
using TravelFoodCms.Models;
using TravelFoodCms.Models.DTOs;

namespace TravelFoodCms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        /// curl -X "GET" https://localhost:5234/api/Orders
        /// <summary>
        /// Returns a list of all Orders
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{OrderDTO},{OrderDTO},...]
        /// </returns>
        /// <example>
        /// GET: api/Orders -> [{OrderDTO},{OrderDTO},...]
        /// </example>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders =  await _context.Orders
                .Include(o => o.Restaurant)
                .Include(o => o.User)
                .ToListAsync();

            var orderDTOs = orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                RestaurantId = o.RestaurantId,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                Status = o.Status,
                SpecialRequests = o.SpecialRequests,
                OrderItems = null 
            }).ToList();

            return orderDTOs;
        }

        // GET: api/Orders/5
        /// curl -X "GET" https://localhost:5234/api/Orders/5
        /// <summary>
        /// Returns a single Order specified by its {id}
        /// </summary>
        /// <param name="id">The order id</param>
        /// <returns>
        /// 200 OK
        /// {OrderDTO}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Orders/5 -> {OrderDTO}
        /// </example>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Restaurant)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

             var orderDTO = new OrderDTO
            {
                OrderId = order.OrderId,
                RestaurantId = order.RestaurantId,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                SpecialRequests = order.SpecialRequests,
                OrderItems = order.OrderItems?.Select(oi => new OrderItemDTO
                {
                    ItemId = oi.ItemId,
                    OrderId = oi.OrderId,
                    ItemName = oi.ItemName,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };

            return orderDTO;
        }

        // GET: api/Orders/ByUser/3
        /// curl -X "GET" https://localhost:5234/api/Orders/ByUser/3
        /// <summary>
        /// Returns all Orders for a specific User
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns>
        /// 200 OK
        /// [{OrderDTO},{OrderDTO},...]
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Orders/ByUser/3 -> [{OrderDTO},{OrderDTO},...]
        /// </example>
        [HttpGet("ByUser/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersByUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var orders = await _context.Orders
                .Include(o => o.Restaurant)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            var orderDTOs = orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                RestaurantId = o.RestaurantId,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                SpecialRequests = o.SpecialRequests,
                OrderItems = null 
            }).ToList();

            return orderDTOs;
        }

        // GET: api/Orders/ByRestaurant/3
        /// curl -X "GET" https://localhost:5234/api/Orders/ByRestaurant/3
        /// <summary>
        /// Returns all Orders for a specific Restaurant
        /// </summary>
        /// <param name="restaurantId">The restaurant id</param>
        /// <returns>
        /// 200 OK
        /// [{OrderDTO},{OrderDTO},...]
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Orders/ByRestaurant/3 -> [{OrderDTO},{OrderDTO},...]
        /// </example>
        [HttpGet("ByRestaurant/{restaurantId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersByRestaurant(int restaurantId)
        {
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurant == null)
            {
                return NotFound("Restaurant not found");
            }

             var orders = await _context.Orders
                .Include(o => o.Restaurant)
                .Where(o => o.RestaurantId == restaurantId)
                .ToListAsync();

            var orderDTOs = orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                RestaurantId = o.RestaurantId,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                SpecialRequests = o.SpecialRequests,
                OrderItems = null 
            }).ToList();

            return orderDTOs;
        }

        // GET: api/Orders/5/OrderItems
        /// curl -X "GET" https://localhost:5234/api/Orders/5/OrderItems
        /// <summary>
        /// Returns all OrderItems associated with a specific Order
        /// </summary>
        /// <param name="id">The order id</param>
        /// <returns>
        /// 200 OK
        /// [{OrderItemDTO},{OrderItemDTO},...]
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Orders/5/OrderItems -> [{OrderItemDTO},{OrderItemDTO},...]
        /// </example>
        [HttpGet("{id}/OrderItems")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<OrderItemDTO>>> GetOrderItems(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var orderItems = await _context.OrderItems
                .Where(oi => oi.OrderId == id)
                .ToListAsync();


            var OrderItemDTOs = orderItems.Select(oi => new OrderItemDTO
            {
                ItemId = oi.ItemId,
                OrderId = oi.OrderId,
                ItemName = oi.ItemName,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList();

            return OrderItemDTOs;
        }

        // POST: api/Orders
        /// curl -X "POST" https://localhost:5234/api/Orders -H "Content-Type: application/json" -d '{"RestaurantId":3,"UserId":2,"Status":"Pending","SpecialRequests":"Extra napkins please"}'
        /// <summary>
        /// Creates a new Order
        /// </summary>
        /// <param name="orderDTO">The order details</param>
        /// <returns>
        /// 201 Created
        /// {OrderDTO}
        /// or
        /// 400 Bad Request
        /// </returns>
        /// <example>
        /// POST: api/Orders -> {OrderDTO}
        /// </example>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderDTO>> CreateOrder(OrderDTO orderDTO)
        {
            var restaurant = await _context.Restaurants.FindAsync(orderDTO.RestaurantId);
            if (restaurant == null)
            {
                return BadRequest("Invalid Restaurant ID");
            }

            var user = await _context.Users.FindAsync(orderDTO.UserId);
            if (user == null)
            {
                return BadRequest("Invalid User ID");
            }

              var order = new Order
            {
                RestaurantId = orderDTO.RestaurantId,
                UserId = orderDTO.UserId,
                OrderDate = DateTime.Now,
                TotalAmount = orderDTO.TotalAmount,
                Status = orderDTO.Status ?? "pending",
                SpecialRequests = orderDTO.SpecialRequests
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            orderDTO.OrderId = order.OrderId;
            orderDTO.OrderDate = order.OrderDate;

            return CreatedAtAction(
                nameof(GetOrder),
                new { id = order.OrderId },
                order);
        }

        // PUT: api/Orders/5
        /// curl -X "PUT" https://localhost:5234/api/Orders/5 -H "Content-Type: application/json" -d '{"OrderId":5,"RestaurantId":3,"UserId":2,"Status":"Completed","TotalAmount":25.99}'
        /// <summary>
        /// Updates an existing Order
        /// </summary>
        /// <param name="id">The order id</param>
        /// <param name="orderDTO">The updated order details</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// PUT: api/Orders/5 -> 204 No Content
        /// </example>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrder(int id, OrderDTO orderDTO)
        {
            if (id != orderDTO.OrderId)
            {
                return BadRequest();
            }

            var restaurant = await _context.Restaurants.FindAsync(orderDTO.RestaurantId);
            if (restaurant == null)
            {
                return BadRequest("Invalid Restaurant ID");
            }

            var user = await _context.Users.FindAsync(orderDTO.UserId);
            if (user == null)
            {
                return BadRequest("Invalid User ID");
            }

             var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);


             if (order == null)
            {
                return NotFound();
            }

            order.RestaurantId = orderDTO.RestaurantId;
            order.UserId = orderDTO.UserId;
            order.TotalAmount = orderDTO.TotalAmount;
            order.Status = orderDTO.Status;
            order.SpecialRequests = orderDTO.SpecialRequests;
            order.OrderDate = orderDTO.OrderDate;
            
             _context.OrderItems.RemoveRange(order.OrderItems);

            if (orderDTO.OrderItems != null)
            {
                foreach (var itemDTO in orderDTO.OrderItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ItemName = itemDTO.ItemName,
                        Quantity = itemDTO.Quantity,
                        UnitPrice = itemDTO.UnitPrice
                    };
                    _context.OrderItems.Add(orderItem);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        // DELETE: api/Orders/5
        /// curl -X "DELETE" https://localhost:5234/api/Orders/5
        /// <summary>
        /// Deletes an Order
        /// </summary>
        /// <param name="id">The order id</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Orders/5 -> 204 No Content
        /// </example>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }

    public class UpdateOrderStatusDTO
    {
        public string Status { get; set; }
    }
}
