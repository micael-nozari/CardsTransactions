using CardsTransaction.Model.EntityModel;
using CardsTransaction.Model.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsTransaction.Model.ServiceModel
{
    public class ProccessAnticipation
    {
        private TransactionContext _context;

        public ProccessAnticipation(TransactionContext context)
        {
            _context = context;
        }

        public async Task ProccessAnticipationRequestAsync(Anticipation anticipation, AnticipationRequest anticipationRequest)
        {
            anticipation.Transactions = await _context.Transactions.Where(t => anticipationRequest.TransactionsIds.Contains(t.Id)).ToListAsync();

            anticipation.AmountRequested = anticipation.Transactions.Sum(t => t.NetAmount);

            foreach (Transaction transaction in anticipation.Transactions)
            {
                transaction.AnticipationId = anticipation.Id;
                transaction.Anticipation = anticipation;
                transaction.Installments = await _context.Installments.Where(i => i.TransactionId == transaction.Id).ToListAsync();

                transaction.Installments.ForEach(i => i.AnticipatedAmount = i.NetAmount - (i.NetAmount * (3.8m / 100)));
            }
        }

        public async Task<Anticipation> GetPendingAnticipationAsync(AnticipationStatus status)
        {
            var anticipation = _context.Anticipations.Where(a => a.Status == status && !a.AnalysisResult.HasValue).FirstOrDefault();

            if (anticipation != null)
                anticipation.Transactions = await _context.Transactions.Where(t => t.AnticipationId == anticipation.Id).ToListAsync();

            return anticipation;
        }

        public async Task BeginAnticipationAnalysisAsync(Anticipation anticipation)
        {            

            foreach (Transaction transaction in anticipation.Transactions)
            {
                transaction.Anticipated = false;
                transaction.Installments = await _context.Installments.Where(i => i.TransactionId == transaction.Id).ToListAsync();
            }

            anticipation.AnalysisStartDate = DateTime.Now;
            anticipation.Status = AnticipationStatus.InAnalysis;            
        }

        public async Task AnalyzeTransaction(Anticipation anticipation, List<AnticipationTransactionAnalysis> anticipationTransaction)
        {
            foreach (var analysis in anticipationTransaction)
            {
                Transaction transaction = anticipation.Transactions.FirstOrDefault(t => t.Id == analysis.TransactionId);
                transaction.Anticipated = analysis.Anticipated;

                if(transaction.Anticipated.Value)
                {
                    transaction.Installments = await _context.Installments.Where(i => i.TransactionId == transaction.Id).ToListAsync();

                    transaction.Installments.ForEach(i => i.TransferredDate = DateTime.Now);
                    anticipation.AnticipatedAmount += transaction.Installments.Sum(i => i.AnticipatedAmount.Value);
                }
            }

            if(!anticipation.Transactions.Any(t => !t.Anticipated.HasValue))
            {
                anticipation.Status = AnticipationStatus.Finished;
                anticipation.AnalysisEndDate = DateTime.Now;

                int refusedCount = anticipation.Transactions.Count(t => !t.Anticipated.Value);
                if (refusedCount == anticipation.Transactions.Count)
                    anticipation.AnalysisResult = AnticipationAnalysisResult.Reproved;
                else if(refusedCount > 0)
                    anticipation.AnalysisResult = AnticipationAnalysisResult.PartiallyApproved;
                else
                    anticipation.AnalysisResult = AnticipationAnalysisResult.Approved;
            }
        }
    }
}
