using FinanceTracker.Api.Data;
using FinanceTracker.Api.Entities;
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
    public class DebtsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DebtsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // GET: api/debts?personName=Коля
        [HttpGet]
        public async Task<ActionResult<List<DebtDto>>> GetAll([FromQuery] string? personName)
        {
            var userId = GetUserId();

            var query = _context.Debts.Where(d => d.UserId == userId);

            if (!string.IsNullOrWhiteSpace(personName))
                query = query.Where(d => d.PersonName.Contains(personName));

            var debts = await query
                .OrderByDescending(d => d.CreatedDate)
                .Select(d => new DebtDto
                {
                    Id = d.Id,
                    Amount = d.Amount,
                    PersonName = d.PersonName,
                    Description = d.Description,
                    IsOwedToMe = d.IsOwedToMe,
                    CreatedDate = d.CreatedDate,
                    DueDate = d.DueDate,
                    IsPaid = d.IsPaid
                })
                .ToListAsync();

            return Ok(debts);
        }

        // GET: api/debts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DebtDto>> GetById(int id)
        {
            var userId = GetUserId();

            var debt = await _context.Debts
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if (debt == null)
                return NotFound();

            return Ok(new DebtDto
            {
                Id = debt.Id,
                Amount = debt.Amount,
                PersonName = debt.PersonName,
                Description = debt.Description,
                IsOwedToMe = debt.IsOwedToMe,
                CreatedDate = debt.CreatedDate,
                DueDate = debt.DueDate,
                IsPaid = debt.IsPaid
            });
        }

        // POST: api/debts
        [HttpPost]
        public async Task<ActionResult<DebtDto>> Create(CreateDebtDto dto)
        {
            var userId = GetUserId();

            var debt = new Debt
            {
                Amount = dto.Amount,
                PersonName = dto.PersonName,
                Description = dto.Description,
                IsOwedToMe = dto.IsOwedToMe,
                CreatedDate = dto.CreatedDate,
                DueDate = dto.DueDate,
                UserId = userId
            };

            _context.Debts.Add(debt);
            await _context.SaveChangesAsync();

            var result = new DebtDto
            {
                Id = debt.Id,
                Amount = debt.Amount,
                PersonName = debt.PersonName,
                Description = debt.Description,
                IsOwedToMe = debt.IsOwedToMe,
                CreatedDate = debt.CreatedDate,
                DueDate = debt.DueDate,
                IsPaid = debt.IsPaid
            };

            return CreatedAtAction(nameof(GetById), new { id = debt.Id }, result);
        }

        // PUT: api/debts/5
        [HttpPut("{id}")]
        public async Task<ActionResult<DebtDto>> Update(int id, UpdateDebtDto dto)
        {
            var userId = GetUserId();

            var debt = await _context.Debts
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if (debt == null)
                return NotFound();

            debt.Amount = dto.Amount;
            debt.PersonName = dto.PersonName;
            debt.Description = dto.Description;
            debt.IsOwedToMe = dto.IsOwedToMe;
            debt.DueDate = dto.DueDate;
            debt.IsPaid = dto.IsPaid;

            await _context.SaveChangesAsync();

            return Ok(new DebtDto
            {
                Id = debt.Id,
                Amount = debt.Amount,
                PersonName = debt.PersonName,
                Description = debt.Description,
                IsOwedToMe = debt.IsOwedToMe,
                CreatedDate = debt.CreatedDate,
                DueDate = debt.DueDate,
                IsPaid = debt.IsPaid
            });
        }

        // DELETE: api/debts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = GetUserId();

            var debt = await _context.Debts
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if (debt == null)
                return NotFound();

            _context.Debts.Remove(debt);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
