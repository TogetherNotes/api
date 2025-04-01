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
    public class rolesController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/roles
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getroles()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var roles = await db.roles
                .Select(r => new
                {
                    r.id,
                    r.name
                })
                .ToListAsync();

            return Ok(roles);
        }


        // GET: api/roles/{id}
        [HttpGet]
        [Route("api/roles/{id}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getroles(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var role = await db.roles
                .Where(r => r.id == id)
                .Select(r => new
                {
                    r.id,
                    r.name
                })
                .FirstOrDefaultAsync();

            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }

        // PUT: api/roles/{id}
        [HttpPut]
        [Route("api/roles/{id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putroles(int id, roles roles)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != roles.id)
                return BadRequest("ID mismatch");

            var existing = await db.roles.FindAsync(id);
            if (existing == null)
                return NotFound();

            // Actualització dels camps editables
            existing.name = roles.name;

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

        // POST: api/roles
        [HttpPost]
        [Route("api/roles")]
        [ResponseType(typeof(roles))]
        public async Task<IHttpActionResult> Postroles(roles roles)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.roles.Add(roles);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = roles.id }, roles);
        }

        // DELETE: api/roles/{id}
        [HttpDelete]
        [Route("api/roles/{id:int}")]
        [ResponseType(typeof(roles))]
        public async Task<IHttpActionResult> Deleteroles(int id)
        {
            var roles = await db.roles.FindAsync(id);
            if (roles == null)
                return NotFound();

            db.roles.Remove(roles);
            await db.SaveChangesAsync();

            return Ok(roles);
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