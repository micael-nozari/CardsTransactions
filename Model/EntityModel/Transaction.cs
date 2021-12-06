using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CardsTransaction.Model.EntityModel
{
    public class Transaction
    {
        public long Id { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? ReprovedAt { get; set; }
        public TransactionStatus Status { get; set; }
        public bool? Anticipated { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal GrossAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal FlatRate { get; set; }
        public int InstallmentNumber { get; set; }
        public string CredidCardNumber { get; set; }

        public long? AnticipationId { get; set; }
        public Anticipation Anticipation { get; set; }

        public List<Installment> Installments { get; set; }

        public Transaction()
        {
            FlatRate = 0.9m;
        }
    }

    public enum TransactionStatus
    {       
        Approved,
        Refused
    }
}
