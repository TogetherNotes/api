using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplicationTgtNotes.Models;

namespace WebApplicationTgtNotes.Controllers
{
    public class incidencesController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/incidences
        [HttpGet]
        [Route("api/incidences")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getincidences()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var incidences = await db.incidences
                .Select(i => new
                {
                    i.app_user_id,
                    i.admin_user_id,
                    i.description,
                    i.status
                })
                .ToListAsync();

            return Ok(incidences);
        }

        // GET: api/incidences/{appUserId}/{adminUserId}
        [HttpGet]
        [Route("api/incidences/{appUserId:int}/{adminUserId:int}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getincidence(int appUserId, int adminUserId)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var incidence = await db.incidences
                .Where(i => i.app_user_id == appUserId && i.admin_user_id == adminUserId)
                .Select(i => new
                {
                    i.app_user_id,
                    i.admin_user_id,
                    i.description,
                    i.status
                })
                .FirstOrDefaultAsync();

            if (incidence == null)
            {
                return NotFound();
            }

            return Ok(incidence);
        }

        // PUT: api/incidences/{app_user_id}/{admin_user_id}
        [HttpPut]
        [Route("api/incidences/{app_user_id:int}/{admin_user_id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putincidences(int app_user_id, int admin_user_id, incidences incidences)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (app_user_id != incidences.app_user_id || admin_user_id != incidences.admin_user_id)
                return BadRequest("ID mismatch");

            var existing = await db.incidences.FindAsync(app_user_id, admin_user_id);
            if (existing == null)
                return NotFound();

            existing.description = incidences.description;
            existing.status = incidences.status;

            try
            {
                await db.SaveChangesAsync();
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/incidences
        [HttpPost]
        [Route("api/incidences")]
        [ResponseType(typeof(incidences))]
        public async Task<IHttpActionResult> Postincidences(incidences incidences)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.incidences.Add(incidences);

            try
            {
                await db.SaveChangesAsync();
                return CreatedAtRoute("DefaultApi", new
                {
                    app_user_id = incidences.app_user_id,
                    admin_user_id = incidences.admin_user_id
                }, incidences);
            }
            catch (DbUpdateException ex)
            {
                return InternalServerError(ex); // o Conflict() si ho vols controlar
            }
        }

        // DELETE: api/incidences/{app_user_id}/{admin_user_id}
        [HttpDelete]
        [Route("api/incidences/{app_user_id:int}/{admin_user_id:int}")]
        [ResponseType(typeof(incidences))]
        public async Task<IHttpActionResult> Deleteincidences(int app_user_id, int admin_user_id)
        {
            var incidence = await db.incidences.FindAsync(app_user_id, admin_user_id);
            if (incidence == null)
                return NotFound();

            db.incidences.Remove(incidence);
            await db.SaveChangesAsync();

            return Ok(incidence);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}