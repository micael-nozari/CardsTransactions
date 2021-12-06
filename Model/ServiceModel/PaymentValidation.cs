using CardsTransaction.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsTransaction.Model.ServiceModel
{
    public static class PaymentValidation
    {
        public static bool IsPaymentValid(Payment payment) => (payment.CardExpirationDate > DateTime.Today) &&
                (payment.CreditCardNumber.Length == 16) &&
                (payment.InstallmentNumber > 0) &&
                (payment.Value > 0);
    }
}
