using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace CardsTransaction.Model.EntityModel
{
    public class TransactionContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<Installment> Installments { get; set; } = null!;
        public DbSet<Anticipation> Anticipations { get; set; }


        public TransactionContext(DbContextOptions<TransactionContext> options)
           : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Password=portal;Persist Security Info=True;User ID=portal;Initial Catalog=CardTransactions;Data Source=DESKTOP-EI6MTVR");
        //}
    }
}
