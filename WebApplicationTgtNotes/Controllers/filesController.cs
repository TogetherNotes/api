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

        // PUT: api/files/{id}/{app_id}
        [HttpPut]
        [Route("api/files/{id:int}/{app_id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putfiles(int id, int app_id, files files)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != files.id || app_id != files.app_id)
                return BadRequest("ID mismatch");

            var existing = await db.files.FindAsync(id, app_id);
            if (existing == null)
                return NotFound();

            // Actualitzar camps
            existing.name = files.name;
            existing.type = files.type;
            existing.date = files.date;

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

        // POST: api/files
        [HttpPost]
        [Route("api/files")]
        [ResponseType(typeof(files))]
        public async Task<IHttpActionResult> Postfiles(files files)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.files.Add(files);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = files.id, app_id = files.app_id }, files);
        }

        // DELETE: api/files/{id}/{app_id}
        [HttpDelete]
        [Route("api/files/{id:int}/{app_id:int}")]
        [ResponseType(typeof(files))]
        public async Task<IHttpActionResult> Deletefiles(int id, int app_id)
        {
            var file = await db.files.FindAsync(id, app_id);
            if (file == null)
                return NotFound();

            db.files.Remove(file);
            await db.SaveChangesAsync();

            return Ok(file);
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