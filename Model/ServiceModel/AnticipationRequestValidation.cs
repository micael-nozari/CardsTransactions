using CardsTransaction.Model.EntityModel;
using CardsTransaction.Model.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsTransaction.Model.ServiceModel
{
    public class AnticipationRequestValidation
    {
        private const string TransactionNotFound = "Transaction not Found";
        private const string TransactionIdNotFound = "Transaction id {0} not found";

        private const string TransactionRefused = "Transaction Refused";
        private const string TransactionIdIsRefused = "Transaction id {0} is Refused, it's not possible anticipate a refused transaction";

        private const string TransactionAnticipated = "Transaction Anticipated";
        private const string TransactionIdIsAlreadyAnticipated = "Transaction id {0} is already requested in another anticipation";

        private const string UnfinishedAnticipation = "Unfinished anticipation";
        private const string ANewAnticipationCannotBeRequested = "A new anticipation cannot be requested until every anticipations be finished";

        private TransactionContext _context;

        public AnticipationRequestValidation(TransactionContext context)
        {
            _context = context;
        }

        public async Task<ValidationProblemDetails> IsAnticipationRequestValidAsync(AnticipationRequest anticipationRequest)
        {
            Dictionary<string, string[]> errors = new Dictionary<string, string[]>();
            foreach (long id in anticipationRequest.TransactionsIds)
            {
                var transaction = await _context.Transactions.FindAsync(id);
                if (transaction == null)
                {
                    if (errors.ContainsKey(TransactionNotFound))
                    {
                        var newArray = new string[errors[TransactionNotFound].Length + 1];
                        errors[TransactionNotFound].CopyTo(newArray, 0);
                        newArray[newArray.Length - 1] = string.Format(TransactionIdNotFound, id);
                        errors[TransactionNotFound] = newArray;
                    }
                    else
                        errors.Add(TransactionNotFound, new string[] { string.Format(TransactionIdNotFound, id) });
                    continue;
                }

                if (transaction.Status == TransactionStatus.Refused)
                {
                    if (errors.ContainsKey(TransactionRefused))
                    {
                        var newArray = new string[errors[TransactionRefused].Length + 1];
                        errors[TransactionRefused].CopyTo(newArray, 0);
                        newArray[newArray.Length - 1] = string.Format(TransactionIdIsRefused, id);
                        errors[TransactionRefused] = newArray;
                    }
                    else
                        errors.Add(TransactionRefused, new string[] { string.Format(TransactionIdIsRefused, id) });
                }

                if (transaction.AnticipationId.HasValue)
                {
                    if (errors.ContainsKey(TransactionAnticipated))
                    {
                        var newArray = new string[errors[TransactionAnticipated].Length + 1];
                        errors[TransactionAnticipated].CopyTo(newArray, 0);
                        newArray[newArray.Length - 1] = string.Format(TransactionIdIsAlreadyAnticipated, id);
                        errors[TransactionAnticipated] = newArray;
                    }
                    else
                        errors.Add(TransactionAnticipated, new string[] { string.Format(TransactionIdIsAlreadyAnticipated, id) });
                }
            }

            if (_context.Anticipations.Any(a => a.Status != AnticipationStatus.Finished))
            {
                if (errors.ContainsKey(UnfinishedAnticipation))                
                    errors[UnfinishedAnticipation].Concat(new string[] { ANewAnticipationCannotBeRequested });
                else
                    errors.Add(UnfinishedAnticipation, new string[] { ANewAnticipationCannotBeRequested });
            }

            
            return errors.Count > 0 ? new ValidationProblemDetails(errors) : null;
        }

        public bool CheckAntecipationTransactions(List<AnticipationTransactionAnalysis> anticipationRequest, Anticipation anticipation)
        {
            foreach (AnticipationTransactionAnalysis analysis in anticipationRequest)
            {
                if (!anticipation.Transactions.Any(t => t.Id == analysis.TransactionId))
                    return false;
            }
            return true;
        }
    }
}
