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
    public class filesController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/files
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getfiles()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var files = await db.files
                .Select(f => new
                {
                    f.id,
                    f.name,
                    f.type,
                    f.date
                })
                .ToListAsync();

            return Ok(files);
        }

        // GET: api/files/{id}
        [HttpGet]
        [Route("api/files/{id}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getfiles(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var file = await db.files
                .Where(f => f.id == id)
                .Select(f => new
                {
                    f.id,
                    f.name,
                    f.type,
                    f.date
                })
                .FirstOrDefaultAsync();

            if (file == null)
            {
                return NotFound();
            }

            return Ok(file);
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