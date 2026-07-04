using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Shared.DTOs
{
    public class DebtDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsOwedToMe { get; set; }
        public DateOnly CreatedDate { get; set; }
        public DateOnly? DueDate { get; set; }
        public bool IsPaid { get; set; }
    }

    public class CreateDebtDto
    {
        public decimal Amount { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsOwedToMe { get; set; }
        public DateOnly CreatedDate { get; set; }
        public DateOnly? DueDate { get; set; }
    }

    public class UpdateDebtDto
    {
        public decimal Amount { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsOwedToMe { get; set; }
        public DateOnly CreatedDate { get; set; }
        public DateOnly? DueDate { get; set; }
        public bool IsPaid { get; set; }
    }
}
