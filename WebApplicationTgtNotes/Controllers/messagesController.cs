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
    public class messagesController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/messages
        public IQueryable<messages> Getmessages()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.messages;
        }

        // GET: api/messages/{id}
        [HttpGet]
        [Route("api/messages/{id}")]
        [ResponseType(typeof(messages))]
        public async Task<IHttpActionResult> Getmessages(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            messages messages = await db.messages.FindAsync(id);
            if (messages == null)
            {
                return NotFound();
            }

            return Ok(messages);
        }

        // PUT: api/messages/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putmessages(int id, messages messages)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != messages.id)
            {
                return BadRequest();
            }

            db.Entry(messages).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!messagesExists(id))
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

        // POST: api/messages
        [ResponseType(typeof(messages))]
        public async Task<IHttpActionResult> Postmessages(messages messages)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.messages.Add(messages);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = messages.id }, messages);
        }

        // DELETE: api/messages/5
        [ResponseType(typeof(messages))]
        public async Task<IHttpActionResult> Deletemessages(int id)
        {
            messages messages = await db.messages.FindAsync(id);
            if (messages == null)
            {
                return NotFound();
            }

            db.messages.Remove(messages);
            await db.SaveChangesAsync();

            return Ok(messages);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool messagesExists(int id)
        {
            return db.messages.Count(e => e.id == id) > 0;
        }
    }
}