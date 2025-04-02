using System;
using System.Data.Entity;
using System.Linq;
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
        [HttpGet]
        [Route("api/spaces")]
        public IQueryable<object> Getspaces()
        {
            db.Configuration.LazyLoadingEnabled = false;

            return db.spaces
                .Select(s => new
                {
                    s.app_user_id,
                    s.capacity,
                    s.zip_code
                });
        }

        // GET: api/spaces/{id}
        [HttpGet]
        [Route("api/spaces/{id}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getspaces(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var space = await db.spaces
                .Where(s => s.app_user_id == id)
                .Select(s => new
                {
                    s.app_user_id,
                    s.capacity,
                    s.zip_code
                })
                .FirstOrDefaultAsync();

            if (space == null)
            {
                return NotFound();
            }

            return Ok(space);
        }

        // PUT: api/spaces/{id}
        [HttpPut]
        [Route("api/spaces/{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putspaces(int id, spaces spaceUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != spaceUpdate.app_user_id)
                return BadRequest("ID mismatch");

            var space = await db.spaces.FindAsync(id);
            if (space == null)
                return NotFound();
            
            space.app_user_id = spaceUpdate.app_user_id;
            space.capacity = spaceUpdate.capacity;
            space.zip_code = spaceUpdate.zip_code;

            try
            {
                await db.SaveChangesAsync();
                return Ok(new { status = "ok", message = "Espai actualitzat correctament" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
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
                        language_id = data.language_id
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
    }
}