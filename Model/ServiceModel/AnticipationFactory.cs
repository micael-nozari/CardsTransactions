using CardsTransaction.Model.EntityModel;
using CardsTransaction.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsTransaction.Model.ServiceModel
{
    public static class AnticipationFactory
    {
        public static Anticipation CreateAnticipation(AnticipationRequest anticipationRequest)
        {
            Anticipation anticipation = new Anticipation()
            {
                RequestDate = DateTime.Now,
                Status = AnticipationStatus.Pending,
                TransactionsIds = string.Join(", ", anticipationRequest.TransactionsIds)
            };

            return anticipation;
        }
    }
}
