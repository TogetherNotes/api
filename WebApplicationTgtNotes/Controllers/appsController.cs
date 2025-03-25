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
        public IQueryable<app> Getapp()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.app;
        }

        // GET: api/apps/5
        [ResponseType(typeof(app))]
        public async Task<IHttpActionResult> Getapp(int id)
        {
            app app = await db.app.FindAsync(id);
            if (app == null)
            {
                return NotFound();
            }

            return Ok(app);
        }

        // PUT: api/apps/5
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

        // DELETE: api/apps/5
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