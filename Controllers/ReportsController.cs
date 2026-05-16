using System.Security.Claims;
using CNPM_Nhom12.Data;
using CNPM_Nhom12.Models;
using CNPM_Nhom12.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CNPM_Nhom12.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly AppDbContext _db;
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        public ReportsController(AppDbContext db) => _db = db;

        public IActionResult Index(int range = 6)
        {
            var fromDate = DateTime.Today.AddMonths(-range + 1);
            var fromStart = new DateTime(fromDate.Year, fromDate.Month, 1);

            var transactions = _db.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == UserId && t.Date >= fromStart)
                .ToList();

            var monthlyData = transactions
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new MonthlyData
                {
                    Label = $"Th.{g.Key.Month}",
                    Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                    Expense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
                })
                .ToList();

            var expenseTx = transactions.Where(t => t.Type == TransactionType.Expense).ToList();
            var totalExpense = expenseTx.Sum(t => t.Amount);

            var categoryBreakdown = expenseTx
                .GroupBy(t => t.Category!)
                .Select(g => new CategorySpend
                {
                    CategoryName = g.Key.Name,
                    Color = g.Key.Color,
                    Amount = g.Sum(t => t.Amount),
                    Percentage = totalExpense > 0
                        ? (double)(g.Sum(t => t.Amount) / totalExpense * 100) : 0
                })
                .OrderByDescending(c => c.Amount)
                .ToList();

            var now = DateTime.Today;
            var budgets = _db.BudgetLimits
                .Include(b => b.Category)
                .Where(b => b.UserId == UserId && b.Month == now.Month && b.Year == now.Year)
                .ToList();

            var txThisMonth = transactions
                .Where(t => t.Type == TransactionType.Expense
                         && t.Date.Month == now.Month
                         && t.Date.Year == now.Year)
                .ToList();

            foreach (var b in budgets)
            {
                b.SpentAmount = txThisMonth
                    .Where(t => t.CategoryId == b.CategoryId)
                    .Sum(t => t.Amount);
            }

            foreach (var c in categoryBreakdown)
            {
                var matched = budgets.FirstOrDefault(b => b.Category?.Name == c.CategoryName);
                if (matched != null)
                {
                    c.LimitAmount = matched.LimitAmount;
                    c.StatusBadge = matched.StatusBadge;
                    c.StatusLabel = matched.StatusLabel;
                }
            }

            return View(new ReportViewModel
            {
                RangeMonths = range,
                MonthlyData = monthlyData,
                CategoryBreakdown = categoryBreakdown,
                BudgetSummary = budgets
            });
        }
    }
}