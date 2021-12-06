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
    [Route("api/PayByCreditCard")]
    [ApiController]
    public class PayByCreditCardController : ControllerBase
    {
        private readonly TransactionContext _context;

        public PayByCreditCardController(TransactionContext context)
        {
            _context = context;
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            return await _context.Transactions.ToListAsync();
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(long id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            transaction.Installments = await _context.Installments.Where(i => i.TransactionId == id).ToListAsync();

            return transaction;
        }
        
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Payment payment)
        {
            if (!PaymentValidation.IsPaymentValid(payment))
                return ValidationProblem("Invalid payment.");

            Transaction transaction = TransactionFactory.CreateTransaction(payment);
            ProccessPayment.ProccessCreditCard(transaction, payment);

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
                       
            //return CreatedAtAction("GetTransaction", new { id = transaction.Id }, transaction);
            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
        }


        private bool TransactionExists(long id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}
