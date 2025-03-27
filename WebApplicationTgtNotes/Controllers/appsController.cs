using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
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
        [HttpGet]
        [Route("api/apps/{id}")]
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
        [HttpPut]
        [Route("api/apps/{id}")]
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

            var existingApp = await db.app.FindAsync(id);
            if (existingApp == null)
            {
                return NotFound();
            }

            // Valors que pot modificar l'usuari
            existingApp.name = app.name;
            existingApp.mail = app.mail;
            existingApp.password = app.password;
            existingApp.role = app.role;
            existingApp.rating = app.rating;
            existingApp.latitude = app.latitude;
            existingApp.longitude = app.longitude;
            existingApp.active = app.active;
            existingApp.language_id = app.language_id;
            existingApp.file_id = app.file_id;
            existingApp.notification_id = app.notification_id;

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

            return Ok(new { status = "ok", message = "L'usuari s'ha actualitzat correctament" });
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