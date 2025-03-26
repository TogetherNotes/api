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
        public IQueryable<rating> Getrating()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.rating;
        }

        // GET: api/ratings/{id}
        [ResponseType(typeof(rating))]
        public async Task<IHttpActionResult> Getrating(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            rating rating = await db.rating.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }

            return Ok(rating);
        }

        // PUT: api/ratings/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putrating(int id, rating rating)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rating.id)
            {
                return BadRequest();
            }

            db.Entry(rating).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ratingExists(id))
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

        // POST: api/ratings
        [ResponseType(typeof(rating))]
        public async Task<IHttpActionResult> Postrating(rating rating)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.rating.Add(rating);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = rating.id }, rating);
        }

        // DELETE: api/ratings/5
        [ResponseType(typeof(rating))]
        public async Task<IHttpActionResult> Deleterating(int id)
        {
            rating rating = await db.rating.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }

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

        private bool ratingExists(int id)
        {
            return db.rating.Count(e => e.id == id) > 0;
        }
    }
}