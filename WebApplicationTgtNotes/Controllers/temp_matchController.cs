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
    public class temp_matchController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/temp_match
        [HttpGet]
        [Route("api/temp_match")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Gettemp_match()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var tempMatches = await db.temp_match
                .Select(tm => new
                {
                    tm.artist_id,
                    tm.space_id,
                    tm.artist_like,
                    tm.space_like,
                    tm.status,
                    tm.request_date
                })
                .ToListAsync();

            return Ok(tempMatches);
        }

        // GET: api/temp_match/{artist_id}/{space_id}
        [HttpGet]
        [Route("api/temp_match/{artist_id:int}/{space_id:int}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Gettemp_match(int artist_id, int space_id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var tempMatch = await db.temp_match
                .Where(tm => tm.artist_id == artist_id && tm.space_id == space_id)
                .Select(tm => new
                {
                    tm.artist_id,
                    tm.space_id,
                    tm.artist_like,
                    tm.space_like,
                    tm.status,
                    tm.request_date
                })
                .FirstOrDefaultAsync();

            if (tempMatch == null)
            {
                return NotFound();
            }

            return Ok(tempMatch);
        }

        // PUT: api/temp_match/{artist_id}/{space_id}
        [HttpPut]
        [Route("api/temp_match/{artist_id:int}/{space_id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Puttemp_match(int artist_id, int space_id, temp_match temp_match)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (artist_id != temp_match.artist_id || space_id != temp_match.space_id)
                return BadRequest("ID mismatch");

            var existing = await db.temp_match.FindAsync(artist_id, space_id);
            if (existing == null)
                return NotFound();

            // Actualitzar camps editables
            existing.artist_like = temp_match.artist_like;
            existing.space_like = temp_match.space_like;
            existing.status = temp_match.status;
            existing.request_date = temp_match.request_date;

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

        // POST: api/temp_match
        [HttpPost]
        [Route("api/temp_match")]
        [ResponseType(typeof(temp_match))]
        public async Task<IHttpActionResult> Posttemp_match(temp_match temp_match)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.temp_match.Add(temp_match);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                var exists = await db.temp_match.FindAsync(temp_match.artist_id, temp_match.space_id);
                if (exists != null)
                    return Conflict();

                throw;
            }

            return CreatedAtRoute("DefaultApi", new { artist_id = temp_match.artist_id, space_id = temp_match.space_id }, temp_match);
        }

        // DELETE: api/temp_match/{artist_id}/{space_id}
        [HttpDelete]
        [Route("api/temp_match/{artist_id:int}/{space_id:int}")]
        [ResponseType(typeof(temp_match))]
        public async Task<IHttpActionResult> Deletetemp_match(int artist_id, int space_id)
        {
            var temp_match = await db.temp_match.FindAsync(artist_id, space_id);
            if (temp_match == null)
                return NotFound();

            db.temp_match.Remove(temp_match);
            await db.SaveChangesAsync();

            return Ok(temp_match);
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