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
    public class artistsController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/artists
        public IQueryable<artists> Getartists()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.artists;
        }

        // GET: api/artists/5
        [ResponseType(typeof(artists))]
        public async Task<IHttpActionResult> Getartists(int id)
        {
            artists artists = await db.artists.FindAsync(id);
            if (artists == null)
            {
                return NotFound();
            }

            return Ok(artists);
        }

        // PUT: api/artists/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putartists(int id, artists artists)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != artists.app_user_id)
            {
                return BadRequest();
            }

            db.Entry(artists).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!artistsExists(id))
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

        // POST: api/artists
        [ResponseType(typeof(artists))]
        public async Task<IHttpActionResult> Postartists(artists artists)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.artists.Add(artists);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (artistsExists(artists.app_user_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = artists.app_user_id }, artists);
        }

        // DELETE: api/artists/5
        [ResponseType(typeof(artists))]
        public async Task<IHttpActionResult> Deleteartists(int id)
        {
            artists artists = await db.artists.FindAsync(id);
            if (artists == null)
            {
                return NotFound();
            }

            db.artists.Remove(artists);
            await db.SaveChangesAsync();

            return Ok(artists);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool artistsExists(int id)
        {
            return db.artists.Count(e => e.app_user_id == id) > 0;
        }
    }
}