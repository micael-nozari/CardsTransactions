using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CardsTransaction.Model.ViewModel
{
    public class Payment
    {
        [Required]
        [StringLength(16)]
        [MinLength(16)]
        [RegularExpression(@"^[0-9]+$")]
        public string CreditCardNumber { get; set; }
        public string BuyerName { get; set; }
        public string CVV { get; set; }
        public DateTime CardExpirationDate { get; set; }
        [Required]
        [Range(1, double.PositiveInfinity)]
        public int InstallmentNumber { get; set; }
        [Required]
        public decimal Value { get; set; }

    }
}
