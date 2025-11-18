using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDataAccessLayer.Entities;

namespace StoreDataAccessLayer.EntitiesConfigurations
{
    public class ReviewReplyConfigurations : IEntityTypeConfiguration<ReviewReply>
    {
        public void Configure(EntityTypeBuilder<ReviewReply> builder)
        {
            builder.ToTable("ReviewReplies");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Reply).HasMaxLength(2000).IsRequired();
            builder.Property(r => r.CreatedAt).IsRequired();
            builder.Property(r => r.IsApproved).HasDefaultValue(true);
            
            builder.HasOne(r => r.Review)
                   .WithMany()
                   .HasForeignKey(r => r.ReviewId)
                   .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(r => r.User)
                   .WithMany()
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}


