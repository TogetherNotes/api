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
        public IQueryable<admin> Getadmin()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.admin;
        }

        // GET: api/admins/5
        [ResponseType(typeof(admin))]
        public async Task<IHttpActionResult> Getadmin(int id)
        {
            admin admin = await db.admin.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            return Ok(admin);
        }

        // PUT: api/admins/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putadmin(int id, admin admin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != admin.id)
            {
                return BadRequest();
            }

            db.Entry(admin).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!adminExists(id))
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

        // POST: api/admins
        [ResponseType(typeof(admin))]
        public async Task<IHttpActionResult> Postadmin(admin admin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.admin.Add(admin);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = admin.id }, admin);
        }

        // DELETE: api/admins/5
        [ResponseType(typeof(admin))]
        public async Task<IHttpActionResult> Deleteadmin(int id)
        {
            admin admin = await db.admin.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

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

        private bool adminExists(int id)
        {
            return db.admin.Count(e => e.id == id) > 0;
        }
    }
}