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
    public class incidencesController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/incidences
        public IQueryable<incidences> Getincidences()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.incidences;
        }

        // GET: api/incidences/5
        [ResponseType(typeof(incidences))]
        public async Task<IHttpActionResult> Getincidences(int id)
        {
            incidences incidences = await db.incidences.FindAsync(id);
            if (incidences == null)
            {
                return NotFound();
            }

            return Ok(incidences);
        }

        // PUT: api/incidences/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putincidences(int id, incidences incidences)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != incidences.app_user_id)
            {
                return BadRequest();
            }

            db.Entry(incidences).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!incidencesExists(id))
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

        // POST: api/incidences
        [ResponseType(typeof(incidences))]
        public async Task<IHttpActionResult> Postincidences(incidences incidences)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.incidences.Add(incidences);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (incidencesExists(incidences.app_user_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = incidences.app_user_id }, incidences);
        }

        // DELETE: api/incidences/5
        [ResponseType(typeof(incidences))]
        public async Task<IHttpActionResult> Deleteincidences(int id)
        {
            incidences incidences = await db.incidences.FindAsync(id);
            if (incidences == null)
            {
                return NotFound();
            }

            db.incidences.Remove(incidences);
            await db.SaveChangesAsync();

            return Ok(incidences);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool incidencesExists(int id)
        {
            return db.incidences.Count(e => e.app_user_id == id) > 0;
        }
    }
}