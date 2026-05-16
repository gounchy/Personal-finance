using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CNPM_Nhom12.Models;

namespace CNPM_Nhom12.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<BudgetLimit> BudgetLimits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Ăn uống", Icon = "ti ti-tools-kitchen-2", Color = "#BA7517", BgColor = "#FAEEDA", Type = TransactionType.Expense },
                new Category { Id = 2, Name = "Di chuyển", Icon = "ti ti-motorbike", Color = "#BA7517", BgColor = "#FAEEDA", Type = TransactionType.Expense },
                new Category { Id = 3, Name = "Giải trí", Icon = "ti ti-device-tv", Color = "#534AB7", BgColor = "#EEEDFE", Type = TransactionType.Expense },
                new Category { Id = 4, Name = "Tiện ích", Icon = "ti ti-plug", Color = "#993C1D", BgColor = "#FCEBEB", Type = TransactionType.Expense },
                new Category { Id = 5, Name = "Giáo dục", Icon = "ti ti-school", Color = "#185FA5", BgColor = "#E6F1FB", Type = TransactionType.Expense },
                new Category { Id = 6, Name = "Lương", Icon = "ti ti-cash", Color = "#0F6E56", BgColor = "#E1F5EE", Type = TransactionType.Income },
                new Category { Id = 7, Name = "Thưởng", Icon = "ti ti-gift", Color = "#0F6E56", BgColor = "#E1F5EE", Type = TransactionType.Income },
                new Category { Id = 8, Name = "Đầu tư", Icon = "ti ti-coins", Color = "#185FA5", BgColor = "#E6F1FB", Type = TransactionType.Income }
            );
        }
    }
}