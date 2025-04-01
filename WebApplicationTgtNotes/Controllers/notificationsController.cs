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
    public class notificationsController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/notifications
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getnotifications()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var notifications = await db.notifications
                .Select(n => new
                {
                    n.id,
                    n.content,
                    n.date,
                    n.app_id
                })
                .ToListAsync();

            return Ok(notifications);
        }

        // GET: api/notifications/{app_id}/{id}
        [HttpGet]
        [Route("api/notifications/{app_id:int}/{id:int}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getnotification(int app_id, int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var notification = await db.notifications
                .Where(n => n.app_id == app_id && n.id == id)
                .Select(n => new
                {
                    n.id,
                    n.content,
                    n.date,
                    n.app_id
                })
                .FirstOrDefaultAsync();

            if (notification == null)
            {
                return NotFound();
            }

            return Ok(notification);
        }

        // GET: api/notifications/by-app/{app_id}
        [HttpGet]
        [Route("api/notifications/by-app/{app_id:int}")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> GetNotificationsByApp(int app_id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var notifications = await db.notifications
                .Where(n => n.app_id == app_id)
                .Select(n => new
                {
                    n.id,
                    n.content,
                    n.date,
                    n.app_id
                })
                .ToListAsync();

            return Ok(notifications);
        }

        // PUT: api/notifications/{id}
        [HttpPut]
        [Route("api/notifications/{id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putnotifications(int id, notifications notifications)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != notifications.id)
                return BadRequest("ID mismatch");

            var existing = await db.notifications.FindAsync(id);
            if (existing == null)
                return NotFound();

            // Actualitzem només els camps modificables
            existing.content = notifications.content;
            existing.date = notifications.date;
            existing.app_id = notifications.app_id;

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

        // POST: api/notifications
        [HttpPost]
        [Route("api/notifications")]
        [ResponseType(typeof(notifications))]
        public async Task<IHttpActionResult> Postnotifications(notifications notifications)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.notifications.Add(notifications);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = notifications.id }, notifications);
        }

        // DELETE: api/notifications/{id}
        [HttpDelete]
        [Route("api/notifications/{id:int}")]
        [ResponseType(typeof(notifications))]
        public async Task<IHttpActionResult> Deletenotifications(int id)
        {
            var notification = await db.notifications.FindAsync(id);
            if (notification == null)
                return NotFound();

            db.notifications.Remove(notification);
            await db.SaveChangesAsync();

            return Ok(notification);
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