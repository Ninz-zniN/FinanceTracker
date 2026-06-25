using FinanceTracker.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Api.Data.Configurations
{
    public class TransactionConfiguration: IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasOne(t => t.User)
                   .WithMany(u => u.Transactions)
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Category)
                   .WithMany(c => c.Transactions)
                   .HasForeignKey(t => t.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
