using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplicationTgtNotes.DTO;
using WebApplicationTgtNotes.Models;

namespace WebApplicationTgtNotes.Controllers
{
    public class artistsController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/artists
        public IQueryable<object> Getartists()
        {
            db.Configuration.LazyLoadingEnabled = false;

            return db.artists
                .Select(a => new
                {
                    a.app_user_id
                });
        }

        // GET: api/artists/{id}
        [HttpGet]
        [Route("api/artists/{id}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getartists(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var artist = await db.artists
                .Where(a => a.app_user_id == id)
                .Select(a => new
                {
                    a.app_user_id
                })
                .FirstOrDefaultAsync();

            if (artist == null)
            {
                return NotFound();
            }

            return Ok(artist);
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

        // POST: api/artists/register
        [HttpPost]
        [Route("api/artists/register")]
        public async Task<IHttpActionResult> RegisterArtist(UserRegisterDTO data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Crear app
                    var newApp = new app
                    {
                        name = data.name,
                        mail = data.mail,
                        password = data.password,
                        role = data.role,
                        rating = data.rating,
                        latitude = data.latitude,
                        longitude = data.longitude,
                        active = data.active,
                        language_id = data.language_id
                    };

                    db.app.Add(newApp);
                    await db.SaveChangesAsync();

                    // Crear artista vinculat a app
                    var newArtist = new artists
                    {
                        app_user_id = newApp.id
                    };

                    db.artists.Add(newArtist);
                    await db.SaveChangesAsync();

                    // Crear associacions amb gèneres
                    if (data.genre_ids != null)
                    {
                        foreach (var genreId in data.genre_ids)
                        {
                            db.artist_genres.Add(new artist_genres
                            {
                                artist_id = newArtist.app_user_id,
                                genre_id = genreId,
                                creation_date = DateTime.Now
                            });
                        }

                        await db.SaveChangesAsync();
                    }

                    transaction.Commit();

                    return Ok(new { status = "ok", app_id = newApp.id });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return InternalServerError(ex);
                }
            }
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