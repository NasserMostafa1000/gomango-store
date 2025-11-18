using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDataLayer.Entities;

namespace StoreDataLayer.EntitiesConfigurations
{
    public class SearchingLogConfigurations : IEntityTypeConfiguration<SearchingLogs>
    {
        public void Configure(EntityTypeBuilder<SearchingLogs> builder)
        {
            builder.HasKey(Sl => Sl.Id);
            builder.Property(Sl => Sl.Id).ValueGeneratedOnAdd().IsRequired();
            builder.Property(Sl => Sl.SearchKeyWord).HasColumnType("NVARCHAR(100)").IsRequired();
            builder.Property(Sl => Sl.SearchDate).HasColumnType("DATETIME");
            builder.Property(Sl => Sl.SearchDate).HasDefaultValueSql("SYSDATETIMEOFFSET() AT TIME ZONE 'Egypt Standard Time'");
            builder.HasOne(Sl => Sl.Client)
                   .WithMany(C => C.ClientSearching)
                   .HasForeignKey(Sl => Sl.ClientId)
                   .OnDelete(DeleteBehavior.SetNull);
            builder.ToTable("SearchingLogs");
        }

    }


}
