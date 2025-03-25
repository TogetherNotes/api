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
    public class filesController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/files
        public IQueryable<files> Getfiles()
        {
            return db.files;
        }

        // GET: api/files/5
        [ResponseType(typeof(files))]
        public async Task<IHttpActionResult> Getfiles(int id)
        {
            files files = await db.files.FindAsync(id);
            if (files == null)
            {
                return NotFound();
            }

            return Ok(files);
        }

        // PUT: api/files/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putfiles(int id, files files)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != files.id)
            {
                return BadRequest();
            }

            db.Entry(files).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!filesExists(id))
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

        // POST: api/files
        [ResponseType(typeof(files))]
        public async Task<IHttpActionResult> Postfiles(files files)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.files.Add(files);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = files.id }, files);
        }

        // DELETE: api/files/5
        [ResponseType(typeof(files))]
        public async Task<IHttpActionResult> Deletefiles(int id)
        {
            files files = await db.files.FindAsync(id);
            if (files == null)
            {
                return NotFound();
            }

            db.files.Remove(files);
            await db.SaveChangesAsync();

            return Ok(files);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool filesExists(int id)
        {
            return db.files.Count(e => e.id == id) > 0;
        }
    }
}