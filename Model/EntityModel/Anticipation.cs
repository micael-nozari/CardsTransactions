using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CardsTransaction.Model.EntityModel
{
    public class Anticipation
    {
        public long Id { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? AnalysisStartDate { get; set; }
        public DateTime? AnalysisEndDate { get; set; }
        public AnticipationStatus Status { get; set; }
        public AnticipationAnalysisResult? AnalysisResult { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountRequested { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal AnticipatedAmount { get; set; }
        public string TransactionsIds { get; set; }
        public List<Transaction> Transactions { get; set; }
    }

    public enum AnticipationStatus
    {
        Pending,
        InAnalysis,
        Finished
    }

    public enum AnticipationAnalysisResult
    {        
        Approved,
        PartiallyApproved,
        Reproved
    }
}
