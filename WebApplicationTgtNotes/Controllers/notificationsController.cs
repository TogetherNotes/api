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
    public class notificationsController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/notifications
        public IQueryable<notifications> Getnotifications()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.notifications;
        }

        // GET: api/notifications/{id}
        [HttpGet]
        [Route("api/notifications/{id}")]
        [ResponseType(typeof(notifications))]
        public async Task<IHttpActionResult> Getnotifications(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            notifications notifications = await db.notifications.FindAsync(id);
            if (notifications == null)
            {
                return NotFound();
            }

            return Ok(notifications);
        }

        // PUT: api/notifications/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putnotifications(int id, notifications notifications)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != notifications.id)
            {
                return BadRequest();
            }

            db.Entry(notifications).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!notificationsExists(id))
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

        // POST: api/notifications
        [ResponseType(typeof(notifications))]
        public async Task<IHttpActionResult> Postnotifications(notifications notifications)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.notifications.Add(notifications);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = notifications.id }, notifications);
        }

        // DELETE: api/notifications/5
        [ResponseType(typeof(notifications))]
        public async Task<IHttpActionResult> Deletenotifications(int id)
        {
            notifications notifications = await db.notifications.FindAsync(id);
            if (notifications == null)
            {
                return NotFound();
            }

            db.notifications.Remove(notifications);
            await db.SaveChangesAsync();

            return Ok(notifications);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool notificationsExists(int id)
        {
            return db.notifications.Count(e => e.id == id) > 0;
        }
    }
}