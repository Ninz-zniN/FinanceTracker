namespace FinanceTracker.Api.Entities
{
    public class Debt
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string PersonName { get; set; } = string.Empty; // Имя человека (кому должен или кто должен)
        public string? Description { get; set; }               // Дополнительные заметки
        public bool IsOwedToMe { get; set; }                   // true — должны мне, false — я должен
        public DateOnly CreatedDate { get; set; }
        public DateOnly? DueDate { get; set; }
        public bool IsPaid { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
