using System.Collections.Generic;
using GoSharpRest.Models.Constants;
using GoSharpRest.Models.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GoSharpRest.Migrationsas
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<GoSharpRest.Models.GoSharpRestContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(GoSharpRest.Models.GoSharpRestContext context)
        {
            var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(context));
            if (!roleManager.RoleExists(RoleName.Admin)) roleManager.Create(new ApplicationRole(RoleName.Admin));
            if (!roleManager.RoleExists(RoleName.Manager)) roleManager.Create(new ApplicationRole(RoleName.Manager));
            if (!roleManager.RoleExists(RoleName.Developer)) roleManager.Create(new ApplicationRole(RoleName.Developer));
            if (!roleManager.RoleExists(RoleName.Customer)) roleManager.Create(new ApplicationRole(RoleName.Customer));

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var admin = userManager.FindByName("admin@admin.com");
            if (admin == null)
            {
                admin = new ApplicationUser()
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    FirstName = "Paul",
                    LastName = "Tsimura",
                    JoinDate = DateTime.Now.AddYears(-1)
                };
                var userResult = userManager.Create(admin, "pas@123");
                if (userResult.Succeeded)
                {
                    var result = userManager.AddToRole(admin.Id, RoleName.Admin);
                }
            }

            var customer = userManager.FindByName("cust@cust.com");
            if (customer == null)
            {
                customer = new ApplicationUser()
                {
                    UserName = "cust@cust.com",
                    Email = "cust@cust.com",
                    FirstName = "Bob",
                    LastName = "Riches",
                    JoinDate = DateTime.Now.AddYears(-1)
                };
                var userResult = userManager.Create(customer, "pas@123");
                if (userResult.Succeeded)
                {
                    var result = userManager.AddToRole(customer.Id, RoleName.Customer);
                }
            }

            /*
            var manager = userManager.FindByName("manager@manager.com");
            if (manager == null)
            {
                manager = new ApplicationUser()
                {
                    UserName = "manager@manager.com",
                    Email = "manager@manager.com",
                    FirstName = "Manager",
                    LastName = "Brown",
                    JoinDate = DateTime.Now.AddYears(-2)
                };
                var userResult = userManager.Create(manager, "pas@123");
                if (userResult.Succeeded)
                {
                    var result = userManager.AddToRole(manager.Id, RoleName.Manager);
                }
            }

            var dev1 = userManager.FindByName("dev1@gosharp.com");
            if (dev1 == null)
            {
                dev1 = new ApplicationUser()
                {
                    UserName = "dev1@gosharp.com",
                    Email = "dev1@gosharp.com",
                    FirstName = "Nancy",
                    LastName = "Brown",
                    JoinDate = DateTime.Now.AddYears(-2)
                };
                var userResult = userManager.Create(dev1, "pas@123");
                if (userResult.Succeeded)
                {
                    var result = userManager.AddToRole(dev1.Id, RoleName.Developer);
                }
            }

            var dev2 = userManager.FindByName("dev2@gosharp.com");
            if (dev2 == null)
            {
                dev2 = new ApplicationUser()
                {
                    UserName = "dev2@gosharp.com",
                    Email = "dev2@gosharp.com",
                    FirstName = "Dominick",
                    LastName = "Percell",
                    JoinDate = DateTime.Now.AddYears(-3)
                };
                var userResult = userManager.Create(dev2, "pas@123");
                if (userResult.Succeeded)
                {
                    var result = userManager.AddToRole(dev2.Id, RoleName.Developer);
                }
            }
            context.SaveChanges();

            /*
            context.SiteTemplates.AddOrUpdate(x => x.Id,
                new SiteTemplate()
                {
                    Id = 1,
                    Title = "Blog Template",
                    Category = SiteTemplateCategory.Blog,
                    ShortDescription = "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium.",
                    Description = "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita distinctio.",
                    Price = 100,
                    ImageUrl = null,
                    EntityStatus = EntityStatus.Available
                },
                new SiteTemplate()
                {
                    Id = 2,
                    Title = "Market Template",
                    Category = SiteTemplateCategory.Market,
                    ShortDescription = "Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur.",
                    Description = "Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur?",
                    Price = 250,
                    ImageUrl = null,
                    EntityStatus = EntityStatus.Available
                });
                */

            var cart = new ShoppingCart()
            {
                User = admin,
                Records = new List<CartRecord>()
            };
            context.Carts.AddOrUpdate(cart);
            context.SaveChanges();
            /*
            var record = new CartRecord()
            {
                ShoppingCart = cart,
                SiteTemplate = context.SiteTemplates.First(),
                Count = 2
            };
            context.CartRecords.Add(record);
            context.SaveChanges();

            cart.Records.Add(record);
            context.SaveChanges();/*

            var order = new Order()
            {
                Description = "Do Smth",
                OrderDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(30),
                OrderStatus = OrderStatus.Initial,
                Customer = dev2
            };
            context.Orders.Add(order);
            context.SaveChanges();

            var orderDetail = new OrderDetail()
            {
                Item = context.SiteTemplates.First(),
                ItemPrice = 100,
                Quantity = 3,
                Order = order
            };
            context.OrderDetails.Add(orderDetail);
            context.SaveChanges();*/

        }
    }
}