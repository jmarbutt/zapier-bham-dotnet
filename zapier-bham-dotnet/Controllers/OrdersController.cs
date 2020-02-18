using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using zapier_bham_dotnet.Data;

namespace zapier_bham_dotnet.Controllers
{
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : Controller
    {
        public OrdersDbContext DbContext { get; }

        public OrdersController(OrdersDbContext dbContext)
        {
            DbContext = dbContext;
        }

        [HttpGet]
        public IQueryable<Order> GetOrders()
        {
            return DbContext.Orders;
        }

        [HttpPost]
        public IActionResult CreateOrder(Order order)
        {
            order.OrderDate = DateTime.Now;
            DbContext.Orders.Add(order);

            DbContext.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [Route("me")]
        public MeModel Me()
        {
            return new MeModel()
            {
                Name = "Jonathan"
            };
        }

        public class MeModel
        {
            public string Name { get; set; }
        }
    }

    
}
