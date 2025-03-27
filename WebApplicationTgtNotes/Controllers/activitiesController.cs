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

        // PUT: api/activities/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putactivity(int id, activity activity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != activity.id)
            {
                return BadRequest();
            }

            db.Entry(activity).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!activityExists(id))
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

        // POST: api/activities
        [ResponseType(typeof(activity))]
        public async Task<IHttpActionResult> Postactivity(activity activity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.activity.Add(activity);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = activity.id }, activity);
        }

        // DELETE: api/activities/5
        [ResponseType(typeof(activity))]
        public async Task<IHttpActionResult> Deleteactivity(int id)
        {
            activity activity = await db.activity.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

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

        private bool activityExists(int id)
        {
            return db.activity.Count(e => e.id == id) > 0;
        }
    }
}