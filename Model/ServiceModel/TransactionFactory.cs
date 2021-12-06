using CardsTransaction.Model.EntityModel;
using CardsTransaction.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsTransaction.Model.ServiceModel
{
    public static class TransactionFactory
    {
        public static Transaction CreateTransaction(Payment payment)
        {
            var transaction = new Transaction
            {
                TransactionDate = DateTime.Now,
                GrossAmount = payment.Value,
                CredidCardNumber = payment.CreditCardNumber.Substring(12),
                InstallmentNumber = payment.InstallmentNumber,
                Anticipated = false,                  
            };

            transaction.Installments = new List<Installment>();

            return transaction;
        }
    }
}
