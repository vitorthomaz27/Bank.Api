using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bank.Api.Models
{
    public class BankingDb
    {
        [Key] 
        public int AccountNumber { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public required string Password { get; set; }

        public decimal Balance { get; set; }

        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public BankingDb() { }

        public BankingDb(int accountNumber, string name, string email, string password, decimal balance)
        {
            AccountNumber = accountNumber;
            Name = name;
            Email = email;
            Password = password;
            Balance = balance;
        }
    }

    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public required string Description { get; set; }

        public decimal Amount { get; set; }

        
        [ForeignKey("BankingDb")]
        public int AccountNumber { get; set; }
    }
}