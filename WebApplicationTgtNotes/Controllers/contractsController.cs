using System;
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
        [HttpGet]
        [Route("api/contracts")]
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
                    c.end_hour,
                    c.title
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
                    c.end_hour,
                    c.title
                })
                .FirstOrDefaultAsync();

            if (contract == null)
            {
                return NotFound();
            }

            return Ok(contract);
        }

        /*
        // GET: api/contracts/by-date?userId={userId}&date={date}
        [HttpGet]
        [Route("api/contracts/by-date")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> GetContractsByDate([FromUri] int userId, [FromUri] DateTime date)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var contracts = await db.contracts
                .Where(c =>
                    (c.artist_id == userId || c.space_id == userId) &&
                    DbFunctions.TruncateTime(c.init_hour) == date.Date
                )
                .Select(c => new
                {
                    c.artist_id,
                    c.space_id,
                    c.meet_type,
                    c.status,
                    c.init_hour,
                    c.end_hour,
                    c.title
                })
                .ToListAsync();

            if (contracts == null || !contracts.Any())
            {
                return NotFound(); // No contracts found for this user on the given date
            }

            return Ok(contracts);
        }
        */

        // GET: api/contracts/by-date?userId={userId}&date={date}
        [HttpGet]
        [Route("api/contracts/by-date")]
        [ResponseType(typeof(IEnumerable<object>))]
        public async Task<IHttpActionResult> GetContractsByDate([FromUri] int userId, [FromUri] DateTime date)
        {
            // Log de los parámetros recibidos
            System.Diagnostics.Debug.WriteLine($"userId: {userId}, date: {date}");

            // Calcular el rango de fechas
            DateTime startDate = date.Date; // Inicio del día (medianoche)
            DateTime endDate = startDate.AddDays(1); // Inicio del día siguiente

            db.Configuration.LazyLoadingEnabled = false;

            var contracts = await db.contracts
                .Where(c =>
                    (c.artist_id == userId || c.space_id == userId) &&
                    c.init_hour >= startDate && c.init_hour < endDate
                )
                .Select(c => new
                {
                    c.artist_id,
                    c.space_id,
                    c.meet_type,
                    c.status,
                    c.init_hour,
                    c.end_hour,
                    c.title
                })
                .ToListAsync();

            if (contracts == null || !contracts.Any())
            {
                return NotFound(); // No contracts found for this user on the given date
            }

            return Ok(contracts);
        }
        // PUT: api/contracts/{artist_id}/{space_id}
        [HttpPut]
        [Route("api/contracts/{artist_id:int}/{space_id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putcontracts(int artist_id, int space_id, contracts contractData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (artist_id != contractData.artist_id || space_id != contractData.space_id)
                return BadRequest("ID mismatch");

            var existing = await db.contracts
                .FirstOrDefaultAsync(c => c.artist_id == artist_id && c.space_id == space_id &&
                                          c.init_hour == contractData.init_hour && c.end_hour == contractData.end_hour && contractData.title == c.title);

            if (existing == null)
                return NotFound();

            existing.meet_type = contractData.meet_type;
            existing.status = contractData.status;

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

        // POST: api/contracts
        [HttpPost]
        [Route("api/contracts")]
        [ResponseType(typeof(contracts))]
        public async Task<IHttpActionResult> Postcontracts(contracts contract)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await db.contracts.FindAsync(contract.artist_id, contract.space_id, contract.init_hour, contract.end_hour, contract.title);
            if (exists != null)
                return Conflict();

            db.contracts.Add(contract);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new
            {
                artist_id = contract.artist_id,
                space_id = contract.space_id,
                init_hour = contract.init_hour,
                end_hour = contract.end_hour,
                title = contract.title
            }, contract);
        }

        // DELETE: api/contracts/{artist_id}/{space_id}/{init_hour}/{end_hour}
        [HttpDelete]
        [Route("api/contracts/{artist_id:int}/{space_id:int}/{init_hour}/{end_hour}")]
        [ResponseType(typeof(contracts))]
        public async Task<IHttpActionResult> Deletecontracts(int artist_id, int space_id, DateTimeOffset init_hour, DateTimeOffset end_hour, string title)
        {
            var contract = await db.contracts.FindAsync(artist_id, space_id, init_hour, end_hour, title);
            if (contract == null)
                return NotFound();

            db.contracts.Remove(contract);
            await db.SaveChangesAsync();

            return Ok(contract);
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