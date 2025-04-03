using Bank.Api.Models;
using Bank.Api.Infraestrutura;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Bank.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController(ConnectionContext context) : ControllerBase
    {
        private readonly Repository _repository = new(context);
        private static readonly Random _random = new();

        [HttpPost("Registro")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_repository.GetByEmail(request.Email) != null)
                return BadRequest("E-mail já cadastrado.");

            int accountNumber = _random.Next(10000, 99999);
            var account = new BankingDb
            {
                AccountNumber = accountNumber,
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                Balance = 0m
            };
            _repository.Add(account);

            return Ok(new { accountNumber, Message = "Conta criada com sucesso!" });
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var account = _repository.GetByEmail(request.Email);
            if (account == null || account.Password != request.Password)
                return Unauthorized("E-mail ou senha inválidos.");

            return Ok(new { account.AccountNumber, Message = "Login realizado com sucesso!" });
        }

        [HttpGet("Saldo/{accountNumber}")]
        public IActionResult GetBalance(int accountNumber)
        {
            var balance = _repository.GetBalance(accountNumber);
            if (_repository.GetByAccountNumber(accountNumber) == null)
                return NotFound("Conta não encontrada.");

            return Ok(new { accountNumber, balance });
        }

        [HttpGet("Extrato/{accountNumber}")]
        public IActionResult GetStatement(int accountNumber)
        {
            var account = _repository.GetByAccountNumber(accountNumber);
            if (account == null)
                return NotFound("Conta não encontrada.");

            return Ok(new { accountNumber, account.Balance, Transactions = account.Transactions });
        }

        [HttpPost("Transferência")]
        public IActionResult Transfer([FromBody] TransferRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _repository.Transfer(request.FromAccountNumber, request.ToAccountNumber, request.Amount);
                return Ok(new { Message = "Transferência realizada com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Adicionar Saldo")]
        public IActionResult AddBalance([FromBody] AddBalanceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _repository.AddBalance(request.AccountNumber, request.Amount);
                return Ok(new { Message = $"Saldo de {request.Amount} adicionado com sucesso à conta {request.AccountNumber}!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class RegisterRequest
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public required string Password { get; set; }
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public required string Password { get; set; }
    }

    public class TransferRequest
    {
        [Required(ErrorMessage = "A conta de origem é obrigatória.")]
        public int FromAccountNumber { get; set; }

        [Required(ErrorMessage = "A conta de destino é obrigatória.")]
        public int ToAccountNumber { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório.")]
        public decimal Amount { get; set; }
    }

    public class AddBalanceRequest
    {
        [Required(ErrorMessage = "O número da conta é obrigatório.")]
        public int AccountNumber { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Amount { get; set; }
    }
}