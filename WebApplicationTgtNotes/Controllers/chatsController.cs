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
    public class chatsController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/chats
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getchats()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var chats = await db.chats
                .Select(c => new
                {
                    c.id,
                    c.date,
                    c.user1_id,
                    c.user2_id
                })
                .ToListAsync();

            return Ok(chats);
        }

        // GET: api/chats/{id}
        [HttpGet]
        [Route("api/chats/{id}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getchats(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var chat = await db.chats
                .Where(c => c.id == id)
                .Select(c => new
                {
                    c.id,
                    c.date,
                    c.user1_id,
                    c.user2_id
                })
                .FirstOrDefaultAsync();

            if (chat == null)
            {
                return NotFound();
            }

            return Ok(chat);
        }

        // PUT: api/chats/{id}
        [HttpPut]
        [Route("api/chats/{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putchats(int id, chats chat)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != chat.id)
                return BadRequest("ID mismatch");

            var existing = await db.chats.FindAsync(id);
            if (existing == null)
                return NotFound();

            // Actualitzem només els camps modificables
            existing.date = chat.date;
            existing.user1_id = chat.user1_id;
            existing.user2_id = chat.user2_id;

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

        // POST: api/chats
        [HttpPost]
        [Route("api/chats")]
        [ResponseType(typeof(chats))]
        public async Task<IHttpActionResult> Postchats(chats chat)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.chats.Add(chat);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = chat.id }, chat);
        }

        // DELETE: api/chats/{id}
        [HttpDelete]
        [Route("api/chats/{id}")]
        [ResponseType(typeof(chats))]
        public async Task<IHttpActionResult> Deletechats(int id)
        {
            var chat = await db.chats.FindAsync(id);
            if (chat == null)
                return NotFound();

            db.chats.Remove(chat);
            await db.SaveChangesAsync();

            return Ok(chat);
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