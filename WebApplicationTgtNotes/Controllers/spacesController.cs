using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplicationTgtNotes.DTO;
using WebApplicationTgtNotes.Models;

namespace WebApplicationTgtNotes.Controllers
{
    public class spacesController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/spaces
        public IQueryable<spaces> Getspaces()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.spaces;
        }

        // GET: api/spaces/{id}
        [HttpGet]
        [Route("api/spaces/{id}")]
        [ResponseType(typeof(spaces))]
        public async Task<IHttpActionResult> Getspaces(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            spaces spaces = await db.spaces.FindAsync(id);
            if (spaces == null)
            {
                return NotFound();
            }

            return Ok(spaces);
        }

        // PUT: api/spaces/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putspaces(int id, spaces spaces)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != spaces.app_user_id)
            {
                return BadRequest();
            }

            db.Entry(spaces).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!spacesExists(id))
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

        // POST: api/spaces/register
        [HttpPost]
        [Route("api/spaces/register")]
        public async Task<IHttpActionResult> RegisterSpace(UserRegisterDTO data)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Crear app
                    var newApp = new app
                    {
                        name = data.name,
                        mail = data.mail,
                        password = data.password,
                        role = data.role,
                        rating = data.rating,
                        latitude = data.latitude,
                        longitude = data.longitude,
                        active = data.active,
                        language_id = data.language_id,
                        file_id = data.file_id,
                        notification_id = data.notification_id
                    };

                    db.app.Add(newApp);
                    await db.SaveChangesAsync();

                    // Crear space vinculat
                    var newSpace = new spaces
                    {
                        app_user_id = newApp.id,
                        capacity = data.capacity,
                        zip_code = data.zip_code
                    };

                    db.spaces.Add(newSpace);
                    await db.SaveChangesAsync();

                    transaction.Commit();

                    return Ok(new { status = "ok", app_id = newApp.id });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return InternalServerError(ex);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool spacesExists(int id)
        {
            return db.spaces.Count(e => e.app_user_id == id) > 0;
        }
    }
}