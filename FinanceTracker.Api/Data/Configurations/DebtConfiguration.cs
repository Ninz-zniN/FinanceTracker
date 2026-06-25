using FinanceTracker.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Api.Data.Configurations
{
    public class DebtConfiguration : IEntityTypeConfiguration<Debt>
    {
        public void Configure(EntityTypeBuilder<Debt> builder)
        {
            builder.HasOne(d => d.User)
               .WithMany(u => u.Debts)
               .HasForeignKey(d => d.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
