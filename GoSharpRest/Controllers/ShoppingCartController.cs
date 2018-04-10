using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using GoSharpRest.Models;
using GoSharpRest.Models.DTO;
using GoSharpRest.Models.Entities;
using Microsoft.AspNet.Identity;

namespace GoSharpRest.Controllers
{
    [Authorize]
    [RoutePrefix("api/cart")]
    public class ShoppingCartController : BaseApiController
    {
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(FullCartReturnModel))]
        public async Task<IHttpActionResult> GetCart()
        {
            var cart = await GetCurrentFullCart();

            if (cart == null)
            {
                return NotFound();
            }

            var model = TheModelFactory.Create(cart);
            return Ok(model);
        }

        [HttpPost]
        [Route("crate")]
        public async Task<IHttpActionResult> CreateCart()
        {
            var user = AppUserManager.Users.Single(u => u.UserName == User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            var cart = await GetCurrentCart();
            if (cart == null)
            {
                cart = new ShoppingCart()
                {
                    User = user,
                    Records = new List<CartRecord>()
                };

                this.DB.Carts.Add(cart);
                await this.DB.SaveChangesAsync();
            }
            
            Uri locationHeader = new Uri(Url.Link("GetFullCart", null));
            return Created(locationHeader, TheModelFactory.Create(cart));
        }

        [HttpPost]
        [Route("items/{itemId}")]
        [ResponseType(typeof(FullCartReturnModel))]
        public async Task<IHttpActionResult> AddItemToCart(int itemId)
        {
            var cart = await GetCurrentFullCart();
            if (cart == null)
            {
                return NotFound();
            }

            var record = cart.Records.Find(r => r.SiteTemplate != null && r.SiteTemplate.Id == itemId);
            if (record == null)
            {
                record = new CartRecord()
                {
                    SiteTemplate = DB.SiteTemplates.Find(itemId),
                    ShoppingCart = cart,
                    Count = 1,
                };
                DB.CartRecords.Add(record);
            }
            else
            {
                record.Count++;
            }
            await DB.SaveChangesAsync();
            return Ok(TheModelFactory.Create(cart));
        }

        [HttpDelete]
        [Route("items/{itemId}")]
        [ResponseType(typeof(FullCartReturnModel))]
        public async Task<IHttpActionResult> RemoveItemFromCart(int itemId)
        {
            var cart = await GetCurrentFullCart();
            if (cart == null)
            {
                return NotFound();
            }

            var record = cart.Records.Find(r => r.SiteTemplate != null && r.SiteTemplate.Id == itemId);
            if (record != null)
            {
                if (record.Count > 1)
                {
                    record.Count--;
                }
                else
                {
                    DB.CartRecords.Remove(record);
                    cart.Records.Remove(record);
                }

                DB.SaveChanges();
            }

            return Ok(TheModelFactory.Create(cart));
        }

        public async Task<ShoppingCart> GetCurrentFullCart()
        {
            return await DB.Carts
                .Include(c => c.Records)
                .Include(c => c.Records.Select(r => r.SiteTemplate))
                .SingleOrDefaultAsync(c => c.User.UserName == User.Identity.Name);
        }

        private async Task<ShoppingCart> GetCurrentCart()
        {
            return await DB.Carts.SingleOrDefaultAsync(c => c.User.UserName == User.Identity.Name);
        }

        private bool CartExists(int id)
        {
            return this.DB.Carts.Count(e => e.Id == id) > 0;
        }
    }
}