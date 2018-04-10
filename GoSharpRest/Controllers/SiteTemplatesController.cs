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
using System.Web.Http.Cors;
using System.Web.Http.Description;
using GoSharpRest.Models;
using GoSharpRest.Models.Constants;
using GoSharpRest.Models.DTO;
using GoSharpRest.Models.Entities;

namespace GoSharpRest.Controllers
{
    [Authorize]
    public class SiteTemplatesController : BaseApiController
    {

        // GET: api/SiteTemplates
        public IQueryable<SiteTemplateBindingModel> GetSiteTemplates()
        {
            var templates = from st in this.DB.SiteTemplates where st.EntityStatus != EntityStatus.Removed
                select new SiteTemplateBindingModel()
                {
                    Id = st.Id,
                    Title = st.Title,
                    Price = st.Price,
                    Category = st.Category,
                    Description = st.Description,
                    ShortDescription = st.ShortDescription,
                    ImageUrl = st.ImageUrl
                };
            return templates;
        }

        // GET: api/SiteTemplates/5
        [ResponseType(typeof(SiteTemplateBindingModel))]
        public async Task<IHttpActionResult> GetSiteTemplate(int id)
        {
            var siteTemplate =
                await this.DB.SiteTemplates
                    .SingleOrDefaultAsync(st => st.Id == id && st.EntityStatus != EntityStatus.Removed);
            if (siteTemplate == null)
            {
                return NotFound();
            }

            return Ok(TheModelFactory.Create(siteTemplate));
        }

        // PUT: api/SiteTemplates/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSiteTemplate(int id, SiteTemplate siteTemplate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != siteTemplate.Id)
            {
                return BadRequest();
            }

            this.DB.Entry(siteTemplate).State = EntityState.Modified;

            try
            {
                await this.DB.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SiteTemplateExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/SiteTemplates
        [ResponseType(typeof(SiteTemplate))]
        public async Task<IHttpActionResult> PostSiteTemplate(SiteTemplate siteTemplate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            siteTemplate.EntityStatus = EntityStatus.Available;
            this.DB.SiteTemplates.Add(siteTemplate);
            await this.DB.SaveChangesAsync();

            var dto = new SiteTemplateBindingModel()
            {
                Id = siteTemplate.Id,
                Title = siteTemplate.Title,
                Price = siteTemplate.Price,
                Category = siteTemplate.Category,
                Description = siteTemplate.Description,
                ShortDescription = siteTemplate.ShortDescription,
                ImageUrl = siteTemplate.ImageUrl,
            };

            return CreatedAtRoute("DefaultApi", new { id = siteTemplate.Id }, dto);
        }

        // DELETE: api/SiteTemplates/5
        [ResponseType(typeof(SiteTemplate))]
        public async Task<IHttpActionResult> DeleteSiteTemplate(int id)
        {
            SiteTemplate siteTemplate = await this.DB.SiteTemplates.FindAsync(id);
            if (siteTemplate == null)
            {
                return NotFound();
            }

            siteTemplate.EntityStatus = EntityStatus.Removed;
            this.DB.Entry(siteTemplate).State = EntityState.Modified;
            await this.DB.SaveChangesAsync();

            return Ok(siteTemplate);
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