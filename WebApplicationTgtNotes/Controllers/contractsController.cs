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
    public class contractsController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/contracts
        public IQueryable<contracts> Getcontracts()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.contracts;
        }

        // GET: api/contracts/{id}
        [HttpGet]
        [Route("api/apps/{id}")]
        [ResponseType(typeof(contracts))]
        public async Task<IHttpActionResult> Getcontracts(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            contracts contracts = await db.contracts.FindAsync(id);
            if (contracts == null)
            {
                return NotFound();
            }

            return Ok(contracts);
        }

        // PUT: api/contracts/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putcontracts(int id, contracts contracts)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contracts.artist_id)
            {
                return BadRequest();
            }

            db.Entry(contracts).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!contractsExists(id))
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

        // POST: api/contracts
        [ResponseType(typeof(contracts))]
        public async Task<IHttpActionResult> Postcontracts(contracts contracts)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.contracts.Add(contracts);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (contractsExists(contracts.artist_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = contracts.artist_id }, contracts);
        }

        // DELETE: api/contracts/5
        [ResponseType(typeof(contracts))]
        public async Task<IHttpActionResult> Deletecontracts(int id)
        {
            contracts contracts = await db.contracts.FindAsync(id);
            if (contracts == null)
            {
                return NotFound();
            }

            db.contracts.Remove(contracts);
            await db.SaveChangesAsync();

            return Ok(contracts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool contractsExists(int id)
        {
            return db.contracts.Count(e => e.artist_id == id) > 0;
        }
    }
}