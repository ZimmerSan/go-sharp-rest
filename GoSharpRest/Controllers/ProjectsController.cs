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
    [RoutePrefix("api/projects")]
    public class ProjectsController : BaseApiController
    {
        [HttpGet]
        [Route("")]
        public IEnumerable<ProjectReturnModel> GetProjects()
        {
            var projects = DB.Projects.Select(TheModelFactory.Create);
            return projects;
        }

        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(ProjectReturnModel))]
        public async Task<IHttpActionResult> GetProject(int id)
        {
            var order = await DB.Projects.SingleOrDefaultAsync(st => st.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(TheModelFactory.Create(order));
        }

        [HttpPost]
        [Route("")]
        [ResponseType(typeof(ProjectReturnModel))]
        public async Task<IHttpActionResult> PostOrder(ProjectReturnModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = DB.Orders.Single(a => a.Id == model.Order.Id);
            var projectManager = AppUserManager.Users.Single(e => e.Id == model.ProjectManager.Id);

            var devIds = model.Developers.Select(e => e.Id);
            var developers = AppUserManager.Users.Where(e => devIds.Contains(e.Id)).ToList();

            var project = new Project()
            {
                Budget = model.Budget,
                Name = model.Name,
                ProjectStatus = ProjectStatus.Initial,
                Order = order,
                ProjectManager = projectManager,
                WorkItems = new List<WorkItem>(),
                Developers = developers,
                CreatedDate = DateTime.Now,
                DueDate = order.DueDate
            };
            DB.Projects.Add(project);
            order.OrderStatus = OrderStatus.InProgress;

            DB.Entry(order).State = EntityState.Modified;
            await DB.SaveChangesAsync();

            return Created("api/projects/" + project.Id, TheModelFactory.Create(project));
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