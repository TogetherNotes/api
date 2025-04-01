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
    public class activitiesController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/activities
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getactivities()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var activities = await db.activity
                .Select(a => new
                {
                    a.id,
                    a.name,
                    a.type,
                    a.description,
                    a.date,
                    a.admin_user_id
                })
                .ToListAsync();

            return Ok(activities);
        }

        // GET: api/activities/{id}
        [HttpGet]
        [Route("api/activities/{id:int}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getactivity(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var activity = await db.activity
                .Where(a => a.id == id)
                .Select(a => new
                {
                    a.id,
                    a.name,
                    a.type,
                    a.description,
                    a.date,
                    a.admin_user_id
                })
                .FirstOrDefaultAsync();

            if (activity == null)
            {
                return NotFound();
            }

            return Ok(activity);
        }

        // PUT: api/activities/{id}
        [HttpPut]
        [Route("api/activities/{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putactivity(int id, activity activity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != activity.id)
                return BadRequest("ID mismatch");

            var existing = await db.activity.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.name = activity.name;
            existing.type = activity.type;
            existing.description = activity.description;
            existing.date = activity.date;
            existing.admin_user_id = activity.admin_user_id;

            try
            {
                await db.SaveChangesAsync();
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (DbUpdateConcurrencyException)
            {
                return InternalServerError();
            }
        }

        // POST: api/activities
        [HttpPost]
        [Route("api/activities")]
        [ResponseType(typeof(activity))]
        public async Task<IHttpActionResult> Postactivity(activity activity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.activity.Add(activity);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = activity.id }, activity);
        }

        // DELETE: api/activities/{id}
        [HttpDelete]
        [Route("api/activities/{id}")]
        [ResponseType(typeof(activity))]
        public async Task<IHttpActionResult> Deleteactivity(int id)
        {
            var activity = await db.activity.FindAsync(id);
            if (activity == null)
                return NotFound();

            db.activity.Remove(activity);
            await db.SaveChangesAsync();

            return Ok(activity);
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