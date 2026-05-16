using System.Security.Claims;
using CNPM_Nhom12.Data;
using CNPM_Nhom12.Models;
using CNPM_Nhom12.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CNPM_Nhom12.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        public HomeController(AppDbContext db) => _db = db;

        public IActionResult Index()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return View("Landing");

            return Dashboard();
        }

        private IActionResult Dashboard()
        {
            var now = DateTime.Today;
            var month = now.Month;
            var year = now.Year;

            var currentMonthTx = _db.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == UserId && t.Date.Month == month && t.Date.Year == year)
                .ToList();

            var prevMonth = now.AddMonths(-1).Month;
            var prevYear = now.AddMonths(-1).Year;
            var prevMonthTx = _db.Transactions
                .Where(t => t.UserId == UserId && t.Date.Month == prevMonth && t.Date.Year == prevYear)
                .ToList();

            var currentIncome = currentMonthTx.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            var currentExpense = currentMonthTx.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
            var prevIncome = prevMonthTx.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            var prevExpense = prevMonthTx.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

            var budgets = _db.BudgetLimits
                .Include(b => b.Category)
                .Where(b => b.UserId == UserId && b.Month == month && b.Year == year)
                .ToList();

            foreach (var b in budgets)
            {
                b.SpentAmount = currentMonthTx
                    .Where(t => t.Type == TransactionType.Expense && t.CategoryId == b.CategoryId)
                    .Sum(t => t.Amount);
            }

            var vm = new DashboardViewModel
            {
                TotalIncome = currentIncome,
                TotalExpense = currentExpense,
                TransactionCount = currentMonthTx.Count,
                IncomeChangePercent = prevIncome > 0 ? (double)((currentIncome - prevIncome) / prevIncome * 100) : 0,
                ExpenseChangePercent = prevExpense > 0 ? (double)((currentExpense - prevExpense) / prevExpense * 100) : 0,
                RecentTransactions = currentMonthTx.OrderByDescending(t => t.Date).Take(5).ToList(),
                BudgetLimits = budgets,
                Month = month,
                Year = year
            };

            return View(vm);
        }

        public IActionResult Error() => View();
    }
}