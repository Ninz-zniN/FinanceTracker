using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Shared.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        public string Type { get; set; } = string.Empty; // "Income" или "Expense"
        public string? Note { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class CreateTransactionDto
    {
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        public string Type { get; set; } = string.Empty; // "Income" или "Expense"
        public string? Note { get; set; }
        public int CategoryId { get; set; }
    }

    public class UpdateTransactionDto
    {
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Note { get; set; }
        public int CategoryId { get; set; }
    }

    public class CategorySummaryDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
