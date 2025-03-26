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
        public IQueryable<chats> Getchats()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.chats;
        }

        // GET: api/chats/{id}
        [ResponseType(typeof(chats))]
        public async Task<IHttpActionResult> Getchats(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            chats chats = await db.chats.FindAsync(id);
            if (chats == null)
            {
                return NotFound();
            }

            return Ok(chats);
        }

        // PUT: api/chats/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putchats(int id, chats chats)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != chats.id)
            {
                return BadRequest();
            }

            db.Entry(chats).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!chatsExists(id))
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

        // POST: api/chats
        [ResponseType(typeof(chats))]
        public async Task<IHttpActionResult> Postchats(chats chats)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.chats.Add(chats);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = chats.id }, chats);
        }

        // DELETE: api/chats/5
        [ResponseType(typeof(chats))]
        public async Task<IHttpActionResult> Deletechats(int id)
        {
            chats chats = await db.chats.FindAsync(id);
            if (chats == null)
            {
                return NotFound();
            }

            db.chats.Remove(chats);
            await db.SaveChangesAsync();

            return Ok(chats);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool chatsExists(int id)
        {
            return db.chats.Count(e => e.id == id) > 0;
        }
    }
}