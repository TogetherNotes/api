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

        // PUT: api/temp_match/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Puttemp_match(int id, temp_match temp_match)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != temp_match.artist_id)
            {
                return BadRequest();
            }

            db.Entry(temp_match).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!temp_matchExists(id))
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

        // POST: api/temp_match
        [ResponseType(typeof(temp_match))]
        public async Task<IHttpActionResult> Posttemp_match(temp_match temp_match)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.temp_match.Add(temp_match);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (temp_matchExists(temp_match.artist_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = temp_match.artist_id }, temp_match);
        }

        // DELETE: api/temp_match/5
        [ResponseType(typeof(temp_match))]
        public async Task<IHttpActionResult> Deletetemp_match(int id)
        {
            temp_match temp_match = await db.temp_match.FindAsync(id);
            if (temp_match == null)
            {
                return NotFound();
            }

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

        private bool temp_matchExists(int id)
        {
            return db.temp_match.Count(e => e.artist_id == id) > 0;
        }
    }
}