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
    public class genresController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/genres
        public IQueryable<object> GetGenres()
        {
            db.Configuration.LazyLoadingEnabled = false;

            return db.genres
                .Select(g => new
                {
                    g.id,
                    g.name
                });
        }

        // GET: api/genres/{id}
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getgenres(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var genre = await db.genres
                .Where(g => g.id == id)
                .Select(g => new
                {
                    g.id,
                    g.name
                })
                .FirstOrDefaultAsync();

            if (genre == null)
            {
                return NotFound();
            }

            return Ok(genre);
        }

        // PUT: api/genres/{id}
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putgenres(int id, genres genres)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != genres.id)
            {
                return BadRequest();
            }

            db.Entry(genres).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!genresExists(id))
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

        // POST: api/genres
        [ResponseType(typeof(genres))]
        public async Task<IHttpActionResult> Postgenres(genres genres)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.genres.Add(genres);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = genres.id }, genres);
        }

        // DELETE: api/genres/{id}
        [ResponseType(typeof(genres))]
        public async Task<IHttpActionResult> Deletegenres(int id)
        {
            genres genres = await db.genres.FindAsync(id);
            if (genres == null)
            {
                return NotFound();
            }

            db.genres.Remove(genres);
            await db.SaveChangesAsync();

            return Ok(genres);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool genresExists(int id)
        {
            return db.genres.Count(e => e.id == id) > 0;
        }
    }
}