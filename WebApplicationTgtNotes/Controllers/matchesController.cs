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
        [HttpGet]
        [Route("api/matches")]
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

        // PUT: api/matches/{artist_id}/{space_id}
        [HttpPut]
        [Route("api/matches/{artist_id:int}/{space_id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putmatches(int artist_id, int space_id, matches matches)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (artist_id != matches.artist_id || space_id != matches.space_id)
                return BadRequest("ID mismatch");

            var existing = await db.matches.FindAsync(artist_id, space_id);
            if (existing == null)
                return NotFound();

            existing.match_date = matches.match_date;

            try
            {
                await db.SaveChangesAsync();
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/matches
        [HttpPost]
        [Route("api/matches")]
        [ResponseType(typeof(matches))]
        public async Task<IHttpActionResult> Postmatches(matches matches)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.matches.Add(matches);

            try
            {
                await db.SaveChangesAsync();
                return CreatedAtRoute("DefaultApi", new { artist_id = matches.artist_id, space_id = matches.space_id }, matches);
            }
            catch (DbUpdateException)
            {
                var exists = await db.matches.FindAsync(matches.artist_id, matches.space_id);
                if (exists != null)
                    return Conflict();

                throw;
            }
        }

        // DELETE: api/matches/{artist_id}/{space_id}
        [HttpDelete]
        [Route("api/matches/{artist_id:int}/{space_id:int}")]
        [ResponseType(typeof(matches))]
        public async Task<IHttpActionResult> Deletematches(int artist_id, int space_id)
        {
            var match = await db.matches.FindAsync(artist_id, space_id);
            if (match == null)
                return NotFound();

            db.matches.Remove(match);
            await db.SaveChangesAsync();

            return Ok(match);
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