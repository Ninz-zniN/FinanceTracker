using FinanceTracker.Api.Data;
using FinanceTracker.Api.Entities;
using FinanceTracker.Shared;
using FinanceTracker.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // GET: api/transactions?from=2026-01-01&to=2026-12-31&categoryId=1
        [HttpGet]
        public async Task<ActionResult<List<TransactionDto>>> GetAll(
            [FromQuery] DateOnly? from,
            [FromQuery] DateOnly? to,
            [FromQuery] int? categoryId)
        {
            var userId = GetUserId();

            var query = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId);

            if (from.HasValue)
                query = query.Where(t => t.Date >= from.Value);

            if (to.HasValue)
                query = query.Where(t => t.Date <= to.Value);

            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId.Value);

            var transactions = await query
                .OrderByDescending(t => t.Date)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Date = t.Date,
                    Type = t.Type.ToString(),
                    Note = t.Note,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category.Name
                })
                .ToListAsync();

            return Ok(transactions);
        }

        // GET: api/transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetById(int id)
        {
            var userId = GetUserId();

            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction == null)
                return NotFound();

            return Ok(new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Date = transaction.Date,
                Type = transaction.Type.ToString(),
                Note = transaction.Note,
                CategoryId = transaction.CategoryId,
                CategoryName = transaction.Category.Name
            });
        }

        // POST: api/transactions
        [HttpPost]
        public async Task<ActionResult<TransactionDto>> Create(CreateTransactionDto dto)
        {
            var userId = GetUserId();

            // Проверяем, что категория существует и принадлежит пользователю
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.UserId == userId);

            if (category == null)
                return BadRequest(new { message = "Category not found" });

            // Парсим тип транзакции из строки
            if (!Enum.TryParse<TransactionType>(dto.Type, out var transactionType))
                return BadRequest(new { message = "Invalid transaction type. Use 'Income' or 'Expense'" });

            var transaction = new Transaction
            {
                Amount = dto.Amount,
                Date = dto.Date,
                Type = transactionType,
                Note = dto.Note,
                UserId = userId,
                CategoryId = dto.CategoryId
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Загружаем категорию для ответа
            await _context.Entry(transaction).Reference(t => t.Category).LoadAsync();

            var result = new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Date = transaction.Date,
                Type = transaction.Type.ToString(),
                Note = transaction.Note,
                CategoryId = transaction.CategoryId,
                CategoryName = transaction.Category.Name
            };

            return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, result);
        }

        // PUT: api/transactions/5
        [HttpPut("{id}")]
        public async Task<ActionResult<TransactionDto>> Update(int id, UpdateTransactionDto dto)
        {
            var userId = GetUserId();

            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction == null)
                return NotFound();

            // Проверяем категорию
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.UserId == userId);

            if (category == null)
                return BadRequest(new { message = "Category not found" });

            if (!Enum.TryParse<TransactionType>(dto.Type, out var transactionType))
                return BadRequest(new { message = "Invalid transaction type. Use 'Income' or 'Expense'" });

            transaction.Amount = dto.Amount;
            transaction.Date = dto.Date;
            transaction.Type = transactionType;
            transaction.Note = dto.Note;
            transaction.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();

            return Ok(new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Date = transaction.Date,
                Type = transaction.Type.ToString(),
                Note = transaction.Note,
                CategoryId = transaction.CategoryId,
                CategoryName = transaction.Category.Name
            });
        }

        // DELETE: api/transactions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = GetUserId();

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction == null)
                return NotFound();

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/transactions/report/monthly?year=2026&month=7
        [HttpGet("report/monthly")]
        public async Task<ActionResult<List<CategorySummaryDto>>> GetMonthlyReport(
            [FromQuery] int year,
            [FromQuery] int month)
        {
            var userId = GetUserId();

            var data = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId
                            && t.Date.Year == year
                            && t.Date.Month == month
                            && t.Type == TransactionType.Expense)
                .GroupBy(t => t.Category.Name)
                .Select(g => new CategorySummaryDto
                {
                    CategoryName = g.Key,
                    TotalAmount = g.Sum(t => t.Amount)
                })
                .ToListAsync();

            return Ok(data);
        }
    }
}
