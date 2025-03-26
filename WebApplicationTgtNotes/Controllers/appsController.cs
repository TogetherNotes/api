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
    public class appsController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/apps
        public IQueryable<object> Getapp()
        {
            db.Configuration.LazyLoadingEnabled = false;

            return db.app
                .Select(a => new
                {
                    a.id,
                    a.name,
                    a.mail,
                    a.password,
                    a.role,
                    a.rating,
                    a.latitude,
                    a.longitude,
                    a.active,
                    a.language_id,
                    a.file_id,
                    a.notification_id,
                });
        }

        // GET: api/apps/{id}
        [ResponseType(typeof(app))]
        public async Task<IHttpActionResult> Getapp(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var app = await db.app
                .Where(a => a.id == id)
                .Select(a => new
                {
                    a.id,
                    a.name,
                    a.mail,
                    a.password,
                    a.role,
                    a.rating,
                    a.latitude,
                    a.longitude,
                    a.active,
                    a.language_id,
                    a.file_id,
                    a.notification_id
                })
                .FirstOrDefaultAsync();

            if (app == null)
            {
                return NotFound();
            }

            return Ok(app);
        }

        // PUT: api/apps/{id}
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putapp(int id, app app)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != app.id)
            {
                return BadRequest();
            }

            db.Entry(app).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!appExists(id))
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

        // POST: api/apps
        [ResponseType(typeof(app))]
        public async Task<IHttpActionResult> Postapp(app app)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.app.Add(app);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = app.id }, app);
        }

        // DELETE: api/apps/}{id}
        [ResponseType(typeof(app))]
        public async Task<IHttpActionResult> Deleteapp(int id)
        {
            app app = await db.app.FindAsync(id);
            if (app == null)
            {
                return NotFound();
            }

            db.app.Remove(app);
            await db.SaveChangesAsync();

            return Ok(app);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool appExists(int id)
        {
            return db.app.Count(e => e.id == id) > 0;
        }
    }
}