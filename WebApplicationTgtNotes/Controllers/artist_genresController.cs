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
    public class artist_genresController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/artist_genres
        public IQueryable<object> Getartist_genres()
        {
            db.Configuration.LazyLoadingEnabled = false;

            return db.artist_genres
                .Select(a => new
                {
                    a.artist_id,
                    a.genre_id,
                    a.creation_date
                });
        }

        // GET: api/artist_genres/{genre_id}
        [HttpGet]
        [Route("api/artist_genres/{id}")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getartist_genres(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var genres = await db.artist_genres
                .Where(a => a.genre_id == id)
                .Select(a => new
                {
                    a.artist_id,
                    a.genre_id,
                    a.creation_date
                })
                .ToListAsync();

            if (genres == null || !genres.Any())
            {
                return NotFound();
            }

            return Ok(genres);
        }


        // PUT: api/artist_genres/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putartist_genres(int id, artist_genres artist_genres)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != artist_genres.artist_id)
            {
                return BadRequest();
            }

            db.Entry(artist_genres).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!artist_genresExists(id))
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

        // POST: api/artist_genres
        [ResponseType(typeof(artist_genres))]
        public async Task<IHttpActionResult> Postartist_genres(artist_genres artist_genres)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.artist_genres.Add(artist_genres);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (artist_genresExists(artist_genres.artist_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = artist_genres.artist_id }, artist_genres);
        }

        // DELETE: api/artist_genres/5
        [ResponseType(typeof(artist_genres))]
        public async Task<IHttpActionResult> Deleteartist_genres(int id)
        {
            artist_genres artist_genres = await db.artist_genres.FindAsync(id);
            if (artist_genres == null)
            {
                return NotFound();
            }

            db.artist_genres.Remove(artist_genres);
            await db.SaveChangesAsync();

            return Ok(artist_genres);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool artist_genresExists(int id)
        {
            return db.artist_genres.Count(e => e.artist_id == id) > 0;
        }
    }
}