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
    public class matchesController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/matches
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getmatches()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var matches = await db.matches
                .Select(m => new
                {
                    m.artist_id,
                    m.space_id,
                    m.match_date
                })
                .ToListAsync();

            return Ok(matches);
        }

        // GET: api/matches/{artist_id}/{space_id}
        [HttpGet]
        [Route("api/matches/{artist_id}/{space_id}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getmatches(int artist_id, int space_id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var match = await db.matches
                .Where(m => m.artist_id == artist_id && m.space_id == space_id)
                .Select(m => new
                {
                    m.artist_id,
                    m.space_id,
                    m.match_date
                })
                .FirstOrDefaultAsync();

            if (match == null)
            {
                return NotFound();
            }

            return Ok(match);
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