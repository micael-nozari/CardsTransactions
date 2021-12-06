using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CardsTransaction.Model.EntityModel;
using CardsTransaction.Model.ViewModel;
using CardsTransaction.Model.ServiceModel;

namespace CardsTransaction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnticipationRequestController : ControllerBase
    {
        private readonly TransactionContext _context;
        private readonly AnticipationRequestValidation _anticipationRequestValidation;
        private readonly ProccessAnticipation _proccessAnticipation;

        public AnticipationRequestController(TransactionContext context)
        {
            _context = context;
            _anticipationRequestValidation = new AnticipationRequestValidation(context);
            _proccessAnticipation = new ProccessAnticipation(context);
        }

        // GET: api/AnticipationRequests/GetAvailableTransactions
        [HttpGet]
        [Route("GetAvailableTransactions")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetAvailableTransactions()
        {
            return await _context.Transactions.Where(t => !t.AnticipationId.HasValue && !t.Anticipated.HasValue && t.Status == TransactionStatus.Approved).ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Anticipation>>> GetAnticipations(AnticipationStatusFilter filter)
        {
            if (filter != null && filter.Status.HasValue)
                return await _context.Anticipations.Where(a => a.Status == filter.Status.Value).ToListAsync();
            else
                return await _context.Anticipations.ToListAsync();
        }

        // GET: api/AnticipationRequests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Anticipation>> GetAnticipation(long id)
        {
            var anticipationRequest = await _context.Anticipations.FindAsync(id);

            if (anticipationRequest == null)
            {
                return NotFound();
            }

            return anticipationRequest;
        }

        // Post: api/AnticipationRequests/AnalyzeTransactions        
        [HttpPost]
        [Route("AnalyzeTransactions")]
        public async Task<IActionResult> AnalyzeTransactions(List<AnticipationTransactionAnalysis> anticipationRequest)
        {
            Anticipation anticipation = await _proccessAnticipation.GetPendingAnticipationAsync(AnticipationStatus.InAnalysis);
            if (anticipation == null)
                return NotFound();
            if (!_anticipationRequestValidation.CheckAntecipationTransactions(anticipationRequest, anticipation))
                return NotFound();

            await _proccessAnticipation.AnalyzeTransaction(anticipation, anticipationRequest);

            _context.Entry(anticipation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnticipationRequestExists(anticipation.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AnticipationRequests
        [HttpPost]
        public async Task<ActionResult<Anticipation>> PostAnticipationRequest(AnticipationRequest anticipationRequest)
        {
            var validations = await _anticipationRequestValidation.IsAnticipationRequestValidAsync(anticipationRequest);
            if (validations != null)
                return ValidationProblem(validations);

            var anticipation = AnticipationFactory.CreateAnticipation(anticipationRequest);

            await _proccessAnticipation.ProccessAnticipationRequestAsync(anticipation, anticipationRequest);

            _context.Anticipations.Add(anticipation);
            await _context.SaveChangesAsync();
            
            //return CreatedAtAction("GetAnticipationRequest", new { id = anticipationRequest.Id }, anticipationRequest);
            return CreatedAtAction(nameof(GetAnticipation), new { id = anticipation.Id }, anticipation);
        }

        // POST: api/AnticipationRequests/BeginAnticipationAnalysis
        [HttpPost]
        [Route("BeginAnticipationAnalysis")]
        public async Task<ActionResult<IEnumerable<Transaction>>> BeginAnticipationAnalysis()
        {
            Anticipation anticipation = await _proccessAnticipation.GetPendingAnticipationAsync(AnticipationStatus.Pending);
            if (anticipation == null)
                return NotFound();
            
            await _proccessAnticipation.BeginAnticipationAnalysisAsync(anticipation);

            _context.Entry(anticipation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnticipationRequestExists(anticipation.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return anticipation.Transactions;
        }


        private bool AnticipationRequestExists(long id)
        {
            return _context.Anticipations.Any(e => e.Id == id);
        }
    }
}
