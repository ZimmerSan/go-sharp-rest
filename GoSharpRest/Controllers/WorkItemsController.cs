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
    [RoutePrefix("api/workItems")]
    public class WorkItemsController : BaseApiController
    {
        [HttpGet]
        [Route("")]
        public IEnumerable<WorkItemReturnModel> GetProjects()
        {
            var projects = DB.WorkItems.Select(TheModelFactory.Create);
            return projects;
        }

        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(WorkItemReturnModel))]
        public async Task<IHttpActionResult> GetProject(int id)
        {
            var order = await DB.WorkItems.SingleOrDefaultAsync(st => st.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(TheModelFactory.Create(order));
        }

        [HttpPut]
        [Route("{id}")]
        [ResponseType(typeof(WorkItemReturnModel))]
        public async Task<IHttpActionResult> UpdateTask(int id, WorkItemReturnModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var workItem = await DB.WorkItems.SingleOrDefaultAsync(st => st.Id == id);
            if (workItem == null)
            {
                return NotFound();
            }

            workItem.Description = model.Description;
            workItem.DueDate = model.DueDate;
            workItem.EstimatedTime = model.EstimatedTime;
            workItem.Name = model.Name;
            workItem.Status = model.Status;

            DB.Entry(workItem).State = EntityState.Modified;
            await DB.SaveChangesAsync();
            return Ok(this.TheModelFactory.Create(workItem));
        }

        [HttpPost]
        [Route("")]
        [ResponseType(typeof(WorkItemReturnModel))]
        public async Task<IHttpActionResult> PostOrder(WorkItemReturnModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var developer = AppUserManager.Users.Single(e => e.Id == model.AssignedDeveloper.Id);
            var project = DB.Projects.Single(a => a.Id == model.ProjectId);

            var workItem = new WorkItem()
            {
                DueDate = model.DueDate,
                AssignedDeveloper = developer,
                Description = model.Description,
                EstimatedTime = model.EstimatedTime,
                Name = model.Name,
                Status = ProjectStatus.Initial,
                Project = project
            };
            DB.WorkItems.Add(workItem);
            DB.Entry(project).State = EntityState.Modified;
            await DB.SaveChangesAsync();

            return Created("api/workItems/" + workItem.Id, TheModelFactory.Create(workItem));
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