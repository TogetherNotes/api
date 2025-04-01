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
    public class languagesController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/languages
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getlanguages()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var languages = await db.languages
                .Select(l => new
                {
                    l.id,
                    l.name
                })
                .ToListAsync();

            return Ok(languages);
        }

        // GET: api/languages/{id}
        [HttpGet]
        [Route("api/languages/{id}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getlanguages(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var language = await db.languages
                .Where(l => l.id == id)
                .Select(l => new
                {
                    l.id,
                    l.name
                })
                .FirstOrDefaultAsync();

            if (language == null)
            {
                return NotFound();
            }

            return Ok(language);
        }

        // PUT: api/languages/{id}
        [HttpPut]
        [Route("api/languages/{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putlanguages(int id, languages languages)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != languages.id)
                return BadRequest("ID mismatch");

            var existing = await db.languages.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.name = languages.name;

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

        // POST: api/languages
        [HttpPost]
        [Route("api/languages")]
        [ResponseType(typeof(languages))]
        public async Task<IHttpActionResult> Postlanguages(languages languages)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.languages.Add(languages);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = languages.id }, languages);
        }

        // DELETE: api/languages/{id}
        [HttpDelete]
        [Route("api/languages/{id}")]
        [ResponseType(typeof(languages))]
        public async Task<IHttpActionResult> Deletelanguages(int id)
        {
            var language = await db.languages.FindAsync(id);
            if (language == null)
                return NotFound();

            db.languages.Remove(language);
            await db.SaveChangesAsync();

            return Ok(language);
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