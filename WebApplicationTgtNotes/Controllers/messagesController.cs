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
    public class messagesController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/messages
        [HttpGet]
        [Route("api/messages")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getmessages()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var messages = await db.messages
                .Select(m => new
                {
                    m.id,
                    m.sender_id,
                    m.content,
                    m.send_at,
                    m.is_read,
                    m.chat_id
                })
                .ToListAsync();

            return Ok(messages);
        }

        // GET: api/messages/{id}
        [HttpGet]
        [Route("api/messages/{id}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getmessages(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var message = await db.messages
                .Where(m => m.id == id)
                .Select(m => new
                {
                    m.id,
                    m.sender_id,
                    m.content,
                    m.send_at,
                    m.is_read,
                    m.chat_id
                })
                .FirstOrDefaultAsync();

            if (message == null)
            {
                return NotFound();
            }

            return Ok(message);
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