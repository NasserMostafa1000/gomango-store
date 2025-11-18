using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDataAccessLayer.Entities;

namespace StoreDataAccessLayer.EntitiesConfigurations
{
    public class CompanyConfigurations : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(c => c.CompanyId);
            builder.Property(c => c.CompanyId).ValueGeneratedOnAdd();

            builder.Property(c => c.CompanyName)
                .HasColumnType("NVARCHAR")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(c => c.RoleId)
                .HasColumnType("TINYINT")
                .IsRequired();

            builder.HasOne(c => c.Role)
                .WithMany()
                .HasForeignKey(c => c.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.ToTable("Companies");
        }
    }
}

