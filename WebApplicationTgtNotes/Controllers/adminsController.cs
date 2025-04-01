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
    public class adminsController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/admins
        public IQueryable<object> Getadmin()
        {
            db.Configuration.LazyLoadingEnabled = false;

            return db.admin
                .Select(a => new
                {
                    a.id,
                    a.name,
                    a.mail,
                    a.password,
                    a.role_id
                });
        }

        // GET: api/admins/{id}
        [HttpGet]
        [Route("api/admins/{id}")]
        [ResponseType(typeof(admin))]
        public async Task<IHttpActionResult> Getadmin(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var admin = await db.admin
                .Where(a => a.id == id)
                .Select(a => new
                {
                    a.id,
                    a.name,
                    a.mail,
                    a.password,
                    a.role_id
                })
                .FirstOrDefaultAsync();

            if (admin == null)
            {
                return NotFound();
            }

                return Ok(admin);
        }

        // PUT: api/admins/{id}
        [HttpPut]
        [Route("api/admins/{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putadmin(int id, admin admin)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != admin.id)
                return BadRequest("ID mismatch");

            var existing = await db.admin.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.name = admin.name;
            existing.mail = admin.mail;
            existing.password = admin.password;
            existing.role_id = admin.role_id;

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

        // POST: api/admins
        [HttpPost]
        [Route("api/admins")]
        [ResponseType(typeof(admin))]
        public async Task<IHttpActionResult> Postadmin(admin admin)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.admin.Add(admin);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = admin.id }, admin);
        }

        // DELETE: api/admins/{id}
        [HttpDelete]
        [Route("api/admins/{id}")]
        [ResponseType(typeof(admin))]
        public async Task<IHttpActionResult> Deleteadmin(int id)
        {
            var admin = await db.admin.FindAsync(id);
            if (admin == null)
                return NotFound();

            db.admin.Remove(admin);
            await db.SaveChangesAsync();

            return Ok(admin);
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