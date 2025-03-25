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
    public class spacesController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/spaces
        public IQueryable<spaces> Getspaces()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.spaces;
        }

        // GET: api/spaces/5
        [ResponseType(typeof(spaces))]
        public async Task<IHttpActionResult> Getspaces(int id)
        {
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

        // POST: api/spaces
        [ResponseType(typeof(spaces))]
        public async Task<IHttpActionResult> Postspaces(spaces spaces)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.spaces.Add(spaces);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (spacesExists(spaces.app_user_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = spaces.app_user_id }, spaces);
        }

        // DELETE: api/spaces/5
        [ResponseType(typeof(spaces))]
        public async Task<IHttpActionResult> Deletespaces(int id)
        {
            spaces spaces = await db.spaces.FindAsync(id);
            if (spaces == null)
            {
                return NotFound();
            }

            db.spaces.Remove(spaces);
            await db.SaveChangesAsync();

            return Ok(spaces);
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