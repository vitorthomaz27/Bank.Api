using Bank.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Bank.Api.Infraestrutura
{
    public class Repository : Interface
    {
        private readonly ConnectionContext _context;

        public Repository(ConnectionContext context)
        {
            _context = context;
        }

        public void Add(BankingDb bankingDb)
        {
            _context.BankingDb.Add(bankingDb);
            _context.SaveChanges();
        }

        public BankingDb? GetByEmail(string email)
        {
            return _context.BankingDb
                .FirstOrDefault(b => b.Email == email);
        }

        public BankingDb? GetByAccountNumber(int accountNumber)
        {
            return _context.BankingDb
                .Include(b => b.Transactions)
                .FirstOrDefault(b => b.AccountNumber == accountNumber);
        }

        public decimal GetBalance(int accountNumber)
        {
            var account = GetByAccountNumber(accountNumber);
            if (account == null)
                throw new Exception("Conta não encontrada.");
            return account.Balance;
        }

        public void Transfer(int fromAccountNumber, int toAccountNumber, decimal amount)
        {
            var fromAccount = GetByAccountNumber(fromAccountNumber);
            var toAccount = GetByAccountNumber(toAccountNumber);

            if (fromAccount == null || toAccount == null)
                throw new Exception("Conta não encontrada.");
            if (fromAccount.Balance < amount)
                throw new Exception("Saldo insuficiente.");

            fromAccount.Balance -= amount;
            toAccount.Balance += amount;

            fromAccount.Transactions.Add(new Transaction
            {
                Date = DateTime.Now,
                Description = $"Transferência para conta {toAccountNumber}",
                Amount = -amount
            });
            toAccount.Transactions.Add(new Transaction
            {
                Date = DateTime.Now,
                Description = $"Transferência recebida da conta {fromAccountNumber}",
                Amount = amount
            });

            _context.SaveChanges();
        }
           
        public void AddBalance(int accountNumber, decimal amount)
        {
            var account = GetByAccountNumber(accountNumber);
            if (account == null)
                throw new Exception("Conta não encontrada.");

            account.Balance += amount;

            
            account.Transactions.Add(new Transaction
            {
                Date = DateTime.Now,
                Description = "Depósito de saldo",
                Amount = amount
            });

            _context.SaveChanges();
        }
    }
}