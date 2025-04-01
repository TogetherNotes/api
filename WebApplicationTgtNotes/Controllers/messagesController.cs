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

        // PUT: api/messages/{id}
        [HttpPut]
        [Route("api/messages/{id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putmessages(int id, messages messages)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != messages.id)
                return BadRequest("ID mismatch");

            var existing = await db.messages.FindAsync(id);
            if (existing == null)
                return NotFound();

            // Actualitzem només els camps modificables
            existing.sender_id = messages.sender_id;
            existing.content = messages.content;
            existing.send_at = messages.send_at;
            existing.is_read = messages.is_read;
            existing.chat_id = messages.chat_id;

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

        // POST: api/messages
        [HttpPost]
        [Route("api/messages")]
        [ResponseType(typeof(messages))]
        public async Task<IHttpActionResult> Postmessages(messages messages)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.messages.Add(messages);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = messages.id }, messages);
        }

        // DELETE: api/messages/{id}
        [HttpDelete]
        [Route("api/messages/{id:int}")]
        [ResponseType(typeof(messages))]
        public async Task<IHttpActionResult> Deletemessages(int id)
        {
            var message = await db.messages.FindAsync(id);
            if (message == null)
                return NotFound();

            db.messages.Remove(message);
            await db.SaveChangesAsync();

            return Ok(message);
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