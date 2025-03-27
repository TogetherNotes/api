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
    public class contractsController : ApiController
    {
        private TgtNotesEntities db = new TgtNotesEntities();

        // GET: api/contracts
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> Getcontracts()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var contracts = await db.contracts
                .Select(c => new
                {
                    c.artist_id,
                    c.space_id,
                    c.meet_type,
                    c.status,
                    c.init_hour,
                    c.end_hour
                })
                .ToListAsync();

            return Ok(contracts);
        }

        // GET: api/contracts/{artist_id}/{space_id}
        [HttpGet]
        [Route("api/contracts/{artist_id}/{space_id}")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Getcontract(int artist_id, int space_id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var contract = await db.contracts
                .Where(c => c.artist_id == artist_id && c.space_id == space_id)
                .Select(c => new
                {
                    c.artist_id,
                    c.space_id,
                    c.meet_type,
                    c.status,
                    c.init_hour,
                    c.end_hour
                })
                .FirstOrDefaultAsync();

            if (contract == null)
            {
                return NotFound();
            }

            return Ok(contract);
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