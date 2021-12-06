using CardsTransaction.Model.EntityModel;
using CardsTransaction.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsTransaction.Model.ServiceModel
{
    public static class ProccessPayment
    {
        public static void ProccessCreditCard(Transaction transaction, Payment payment)
        {
            if (payment.CreditCardNumber.StartsWith("5999"))
            {
                transaction.Status = TransactionStatus.Refused;
                transaction.ReprovedAt = DateTime.Now;
            }                    
            else
            {
                transaction.Status = TransactionStatus.Approved;
                transaction.ApprovedAt = DateTime.Now;

                transaction.NetAmount = payment.Value - transaction.FlatRate;                

                GenerateInstallments(transaction);
            }
        }

        public static void GenerateInstallments(Transaction transaction)
        {
            for (int i = 1; i <= transaction.InstallmentNumber; i++)
            {
                Installment installment = new Installment()
                {
                    TransactionId = transaction.Id,
                    Transaction = transaction,
                    NumberInstallment = i,
                    GrossAmount = transaction.GrossAmount / transaction.InstallmentNumber,
                    NetAmount = transaction.NetAmount / transaction.InstallmentNumber,
                    ReceiptDate = transaction.TransactionDate.HasValue ? 
                        transaction.TransactionDate.Value.AddDays(30 * i) : 
                        DateTime.Now.AddDays(30 * i)
                };

                transaction.Installments.Add(installment);
            }
        }
    }
}
