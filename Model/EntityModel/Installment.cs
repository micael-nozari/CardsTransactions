using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardsTransaction.Model.EntityModel
{
    public class Installment
    {
        public long Id { get; set; }
        public Transaction Transaction { get; set; }
        public long TransactionId { get; set; }
        public int NumberInstallment { get; set; }
        [Column(TypeName = "decimal(18,2)")]        
        public decimal GrossAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? AnticipatedAmount { get; set; }
        public DateTime ReceiptDate { get; set; }
        public DateTime? TransferredDate { get; set; }

        
    }
}
