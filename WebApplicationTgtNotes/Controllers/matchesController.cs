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
    public class matchesController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/matches
        public IQueryable<matches> Getmatches()
        {
            db.Configuration.LazyLoadingEnabled = false;

            return db.matches;
        }

        // GET: api/matches/{id}
        [HttpGet]
        [Route("api/apps/{id}")]
        [ResponseType(typeof(matches))]
        public async Task<IHttpActionResult> Getmatches(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            matches matches = await db.matches.FindAsync(id);
            if (matches == null)
            {
                return NotFound();
            }

            return Ok(matches);
        }

        // PUT: api/matches/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putmatches(int id, matches matches)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != matches.artist_id)
            {
                return BadRequest();
            }

            db.Entry(matches).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!matchesExists(id))
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

        // POST: api/matches
        [ResponseType(typeof(matches))]
        public async Task<IHttpActionResult> Postmatches(matches matches)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.matches.Add(matches);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (matchesExists(matches.artist_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = matches.artist_id }, matches);
        }

        // DELETE: api/matches/5
        [ResponseType(typeof(matches))]
        public async Task<IHttpActionResult> Deletematches(int id)
        {
            matches matches = await db.matches.FindAsync(id);
            if (matches == null)
            {
                return NotFound();
            }

            db.matches.Remove(matches);
            await db.SaveChangesAsync();

            return Ok(matches);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool matchesExists(int id)
        {
            return db.matches.Count(e => e.artist_id == id) > 0;
        }
    }
}