using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace StoreDataAccessLayer
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var config = BuildConfiguration();
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var conn = config.GetConnectionString("ConnStr")
                       ?? Environment.GetEnvironmentVariable("ConnStr")
                       ?? "Server=localhost;Database=GomangoShop;Trusted_Connection=True;TrustServerCertificate=True;";
            optionsBuilder.UseSqlServer(conn);
            return new AppDbContext(optionsBuilder.Options);
        }

        private static IConfiguration BuildConfiguration()
        {
            // Try to locate OnlineStoreAPIs/appsettings.json relative to this project
            var current = Directory.GetCurrentDirectory();
            string? root = current;
            for (int i = 0; i < 5 && root != null; i++)
            {
                var candidate = Path.Combine(root, "OnlineStoreAPIs", "appsettings.json");
                if (File.Exists(candidate))
                {
                    return new ConfigurationBuilder()
                        .SetBasePath(Path.GetDirectoryName(candidate)!)
                        .AddJsonFile("appsettings.json", optional: false)
                        .Build();
                }
                root = Directory.GetParent(root)?.FullName;
            }
            // Fallback to empty configuration
            return new ConfigurationBuilder().Build();
        }
    }
}


