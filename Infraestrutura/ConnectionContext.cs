using Bank.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Bank.Api.Infraestrutura
{
    public class ConnectionContext : DbContext
    {
        public DbSet<BankingDb> BankingDb { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public ConnectionContext(DbContextOptions<ConnectionContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<BankingDb>()
                .HasMany(b => b.Transactions)
                .WithOne()
                .HasForeignKey("AccountNumber");

            
            modelBuilder.Entity<BankingDb>()
                .Property(b => b.Balance)
                .HasPrecision(18, 4);

            
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 4);

            
            modelBuilder.Entity<BankingDb>()
                .Property(b => b.AccountNumber)
                .ValueGeneratedNever();
        }
    }
}