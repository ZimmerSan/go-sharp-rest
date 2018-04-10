using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using GoSharpRest.Models.Constants;
using GoSharpRest.Models.DTO;
using GoSharpRest.Models.Entities;

namespace GoSharpRest.Controllers
{
    [Authorize]
    [RoutePrefix("api/orders")]
    public class OrdersController : BaseApiController
    {
        [HttpGet]
        [Route("")]
        public IEnumerable<OrderReturnModel> GetOrders()
        {
            var orders = DB.Orders.Select(TheModelFactory.Create);
            return orders;
        }

        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(OrderReturnModel))]
        public async Task<IHttpActionResult> GetOrder(int id)
        {
            var order = await DB.Orders .SingleOrDefaultAsync(st => st.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(TheModelFactory.Create(order));
        }

        [HttpPut]
        [Route("{id}")]
        [ResponseType(typeof(OrderReturnModel))]
        public async Task<IHttpActionResult> ChangeOrderStatus(int id, string status)
        {
            var order = await DB.Orders.SingleOrDefaultAsync(st => st.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatus = status;
            DB.Entry(order).State = EntityState.Modified;
            DB.SaveChanges();
            return Ok(TheModelFactory.Create(order));
        }

        [HttpPost]
        [Route("")]
        [ResponseType(typeof(OrderReturnModel))]
        public async Task<IHttpActionResult> PostOrder(OrderReturnModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = AppUserManager.Users.Single(u => u.UserName == User.Identity.Name);
            if (customer == null)
            {
                return NotFound();
            }

            var order = new Order
            {
                Description = model.Description,
                DueDate = model.DueDate,
                OrderDate = DateTime.Now,
                OrderStatus = OrderStatus.Initial,
                Customer = customer,
                OrderDetails = new List<OrderDetail>()
            };
            DB.Orders.Add(order);
            await DB.SaveChangesAsync();

            var cart = await GetCurrentFullCart();
            cart.Records.ForEach(rec =>
            {
                var orderDetail = new OrderDetail()
                {
                    Item = rec.SiteTemplate,
                    ItemPrice = rec.SiteTemplate.Price,
                    Quantity = rec.Count,
                    Order = order
                };
                DB.OrderDetails.Add(orderDetail);
            });
            DB.CartRecords.RemoveRange(cart.Records);
            await DB.SaveChangesAsync();

            return Created("api/orders/" + order.Id, TheModelFactory.Create(order));
        }

        private async Task<ShoppingCart> GetCurrentFullCart()
        {
            return await DB.Carts
                .Include(c => c.Records)
                .Include(c => c.Records.Select(r => r.SiteTemplate))
                .SingleOrDefaultAsync(c => c.User.UserName == User.Identity.Name);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DB.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SiteTemplateExists(int id)
        {
            return this.DB.SiteTemplates.Count(e => e.Id == id) > 0;
        }
    }
}