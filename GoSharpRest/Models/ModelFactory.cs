using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;
using GoSharpRest.Infrastructure;
using GoSharpRest.Models.DTO;
using GoSharpRest.Models.Entities;

namespace GoSharpRest.Models
{
    public class ModelFactory
    {
        private UrlHelper _UrlHelper;
        private ApplicationUserManager _AppUserManager;

        public ModelFactory(HttpRequestMessage request, ApplicationUserManager appUserManager)
        {
            _UrlHelper = new UrlHelper(request);
            _AppUserManager = appUserManager;
        }

        public UserReturnModel Create(ApplicationUser appUser)
        {
            return new UserReturnModel
            {
                Url = _UrlHelper.Link("GetUserById", new { id = appUser.Id }),
                Id = appUser.Id,
                UserName = appUser.UserName,
                FullName = string.Format("{0} {1}", appUser.FirstName, appUser.LastName),
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Email = appUser.Email,
                JoinDate = appUser.JoinDate,
                Roles = _AppUserManager.GetRolesAsync(appUser.Id).Result,
                Claims = _AppUserManager.GetClaimsAsync(appUser.Id).Result,
                ImageUrl = appUser.ImageUrl,
                Description = appUser.Description,
            };
        }

        public UserReturnModelFull CreateFull(ApplicationUser appUser)
        {
            return new UserReturnModelFull
            {
                Url = _UrlHelper.Link("GetUserById", new { id = appUser.Id }),
                Id = appUser.Id,
                UserName = appUser.UserName,
                FullName = string.Format("{0} {1}", appUser.FirstName, appUser.LastName),
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Email = appUser.Email,
                JoinDate = appUser.JoinDate,
                Roles = _AppUserManager.GetRolesAsync(appUser.Id).Result,
                Claims = _AppUserManager.GetClaimsAsync(appUser.Id).Result,
                ImageUrl = appUser.ImageUrl,
                Description = appUser.Description,
                ManagedProjects = appUser.ManagedProjects.Select(Create).ToList(),
                AssignedProjects = appUser.AssignedProjects.Select(Create).ToList(),
                AssignedTasks = appUser.AssignedTasks.Select(Create).ToList(),
                AssignedOrders = appUser.AssignedOrders.Select(Create).ToList(),
                CustomersProjects = appUser.AssignedOrders.Select(order => order.Project != null ? Create(order.Project) : null).ToList(),
            };
        }

        public RoleReturnModel Create(ApplicationRole appRole)
        {
            return new RoleReturnModel
            {
                Url = _UrlHelper.Link("GetRoleById", new { id = appRole.Id }),
                Id = appRole.Id,
                Name = appRole.Name
            };
        }

        public FullCartReturnModel Create(ShoppingCart cart)
        {
            return new FullCartReturnModel
            {
                UserId = cart.User.Id,
                CartRecords = cart.Records.Select(Create).ToList()
            };
        }

        public CartRecordReturnModel Create(CartRecord record)
        {
            return new CartRecordReturnModel()
            {
                SiteTemplate = Create(record.SiteTemplate),
                Id = record.Id,
                Count = record.Count
            };
        }

        public SiteTemplateBindingModel Create(SiteTemplate siteTemplate)
        {
            return new SiteTemplateBindingModel
            {
                Id = siteTemplate.Id,
                Title = siteTemplate.Title,
                Price = siteTemplate.Price,
                Category = siteTemplate.Category,
                Description = siteTemplate.Description,
                ShortDescription = siteTemplate.ShortDescription,
                ImageUrl = siteTemplate.ImageUrl
            };
        }

        public OrderReturnModel Create(Order order)
        {
            return new OrderReturnModel()
            {
                Id = order.Id,
                Customer = Create(order.Customer),
                OrderDetails = order.OrderDetails.Select(Create).ToList(),
                OrderDate = order.OrderDate,
                DueDate = order.DueDate,
                OrderStatus = order.OrderStatus,
                Description = order.Description
            };
        }

        public OrderDetailReturnModel Create(OrderDetail orderDetail)
        {
            return new OrderDetailReturnModel()
            {
                Id = orderDetail.Id,
                Quantity = orderDetail.Quantity,
                ItemPrice = orderDetail.ItemPrice,
                Item = Create(orderDetail.Item)
            };
        }

        public ProjectReturnModel Create(Project project)
        {
            return new ProjectReturnModel()
            {
                Id = project.Id,
                Budget = project.Budget,
                Name = project.Name,
                ProjectStatus = project.ProjectStatus,
                Order = Create(project.Order),
                ProjectManager = Create(project.ProjectManager),
                WorkItems = project.WorkItems.Select(Create).ToList(),
                Developers = project.Developers.Select(Create).ToList(),
                DueDate = project.DueDate,
                CreatedDate = project.CreatedDate
            };
        }

        public WorkItemReturnModel Create(WorkItem workItem)
        {
            return new WorkItemReturnModel()
            {
                Id = workItem.Id,
                Name = workItem.Name,
                DueDate = workItem.DueDate,
                Description = workItem.Description,
                Status = workItem.Status,
                AssignedDeveloper = Create(workItem.AssignedDeveloper),
                EstimatedTime = workItem.EstimatedTime,
                ProjectId = workItem.Project.Id
            };
        }
    }

    public class UserReturnModel
    {
        public string Url { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime JoinDate { get; set; }
        public IList<string> Roles { get; set; }
        public IList<System.Security.Claims.Claim> Claims { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }

    public class UserReturnModelFull : UserReturnModel
    {
        public List<ProjectReturnModel> ManagedProjects { get; set; }
        public List<ProjectReturnModel> AssignedProjects { get; set; }
        public List<OrderReturnModel> AssignedOrders { get; set; }
        public List<ProjectReturnModel> CustomersProjects { get; set; }
        public List<WorkItemReturnModel> AssignedTasks { get; set; }
    }

    public class RoleReturnModel
    {
        public string Url { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }
}