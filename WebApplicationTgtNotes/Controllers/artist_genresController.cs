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


        // PUT: api/artist_genres/{artist_id}/{genre_id}
        [HttpPut]
        [Route("api/artist_genres/{artist_id:int}/{genre_id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putartist_genres(int artist_id, int genre_id, artist_genres artist_genres)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (artist_id != artist_genres.artist_id || genre_id != artist_genres.genre_id)
                return BadRequest("ID mismatch");

            var existing = await db.artist_genres.FindAsync(artist_id, genre_id);
            if (existing == null)
                return NotFound();

            existing.creation_date = artist_genres.creation_date;

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

        // POST: api/artist_genres
        [HttpPost]
        [Route("api/artist_genres")]
        [ResponseType(typeof(artist_genres))]
        public async Task<IHttpActionResult> Postartist_genres(artist_genres artist_genres)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Comprovar si ja existeix
            var existing = await db.artist_genres.FindAsync(artist_genres.artist_id, artist_genres.genre_id);
            if (existing != null)
                return Conflict();

            db.artist_genres.Add(artist_genres);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new
            {
                artist_id = artist_genres.artist_id,
                genre_id = artist_genres.genre_id
            }, artist_genres);
        }

        // DELETE: api/artist_genres/{artist_id}/{genre_id}
        [HttpDelete]
        [Route("api/artist_genres/{artist_id:int}/{genre_id:int}")]
        [ResponseType(typeof(artist_genres))]
        public async Task<IHttpActionResult> Deleteartist_genres(int artist_id, int genre_id)
        {
            var artist_genres = await db.artist_genres.FindAsync(artist_id, genre_id);
            if (artist_genres == null)
                return NotFound();

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
    }
}