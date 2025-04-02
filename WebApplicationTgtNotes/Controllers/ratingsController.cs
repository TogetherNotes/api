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
    public class ratingsController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/ratings
        [HttpGet]
        [Route("api/ratings")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getrating()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var ratings = await db.rating
                .Select(r => new
                {
                    r.id,
                    r.rating1,
                    r.artist_id,
                    r.space_id
                })
                .ToListAsync();

            return Ok(ratings);
        }

        // GET: api/ratings/{id}
        [HttpGet]
        [Route("api/ratings/{id}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getrating(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var rating = await db.rating
                .Where(r => r.id == id)
                .Select(r => new
                {
                    r.id,
                    r.rating1,
                    r.artist_id,
                    r.space_id
                })
                .FirstOrDefaultAsync();

            if (rating == null)
            {
                return NotFound();
            }

            return Ok(rating);
        }

        // PUT: api/ratings/{id}
        [HttpPut]
        [Route("api/ratings/{id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putrating(int id, rating rating)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != rating.id)
                return BadRequest("ID mismatch");

            var existing = await db.rating.FindAsync(id);
            if (existing == null)
                return NotFound();

            // Actualitzem només els camps modificables
            existing.rating1 = rating.rating1;
            existing.artist_id = rating.artist_id;
            existing.space_id = rating.space_id;

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

        // POST: api/ratings
        [HttpPost]
        [Route("api/ratings")]
        [ResponseType(typeof(rating))]
        public async Task<IHttpActionResult> Postrating(rating rating)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.rating.Add(rating);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = rating.id }, rating);
        }

        // DELETE: api/ratings/{id}
        [HttpDelete]
        [Route("api/ratings/{id:int}")]
        [ResponseType(typeof(rating))]
        public async Task<IHttpActionResult> Deleterating(int id)
        {
            var rating = await db.rating.FindAsync(id);
            if (rating == null)
                return NotFound();

            db.rating.Remove(rating);
            await db.SaveChangesAsync();

            return Ok(rating);
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