namespace FinanceTracker.Api.Entities
{
    public class Debt
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsOwedToMe { get; set; } // true – должны мне, false – я должен
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
