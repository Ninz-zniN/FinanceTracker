namespace FinanceTracker.Api.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Навигационные свойства
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Debt> Debts { get; set; } = new List<Debt>();
    }
}
