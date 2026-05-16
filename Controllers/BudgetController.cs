using System.Security.Claims;
using CNPM_Nhom12.Data;
using CNPM_Nhom12.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CNPM_Nhom12.Controllers
{
    [Authorize]
    public class BudgetController : Controller
    {
        private readonly AppDbContext _db;
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        public BudgetController(AppDbContext db) => _db = db;

        public IActionResult Index()
        {
            var now = DateTime.Today;
            var month = now.Month;
            var year = now.Year;

            var budgets = _db.BudgetLimits
                .Include(b => b.Category)
                .Where(b => b.UserId == UserId && b.Month == month && b.Year == year)
                .ToList();

            var txThisMonth = _db.Transactions
                .Where(t => t.UserId == UserId
                         && t.Type == TransactionType.Expense
                         && t.Date.Month == month
                         && t.Date.Year == year)
                .ToList();

            foreach (var b in budgets)
            {
                b.SpentAmount = txThisMonth
                    .Where(t => t.CategoryId == b.CategoryId)
                    .Sum(t => t.Amount);
            }

            return View(budgets);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _db.Categories
                .Where(c => c.Type == TransactionType.Expense)
                .ToList();

            return View(new BudgetLimit
            {
                Month = DateTime.Today.Month,
                Year = DateTime.Today.Year
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(BudgetLimit budget)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("Category");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _db.Categories
                    .Where(c => c.Type == TransactionType.Expense).ToList();
                return View(budget);
            }

            var exists = _db.BudgetLimits.Any(b =>
                b.UserId == UserId &&
                b.CategoryId == budget.CategoryId &&
                b.Month == budget.Month &&
                b.Year == budget.Year);

            if (exists)
            {
                ModelState.AddModelError("", "Danh mục này đã có hạn mức trong tháng đó rồi.");
                ViewBag.Categories = _db.Categories
                    .Where(c => c.Type == TransactionType.Expense).ToList();
                return View(budget);
            }

            try
            {
                budget.UserId = UserId;
                _db.BudgetLimits.Add(budget);
                _db.SaveChanges();
                TempData["Success"] = "Đã thiết lập hạn mức!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra, vui lòng thử lại.");
                ViewBag.Categories = _db.Categories
                    .Where(c => c.Type == TransactionType.Expense).ToList();
                return View(budget);
            }
        }

        public IActionResult Edit(int id)
        {
            var b = _db.BudgetLimits.FirstOrDefault(x => x.Id == id && x.UserId == UserId);
            if (b == null) return NotFound();

            ViewBag.Categories = _db.Categories
                .Where(c => c.Type == TransactionType.Expense).ToList();

            return View(b);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, BudgetLimit budget)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("Category");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _db.Categories
                    .Where(c => c.Type == TransactionType.Expense).ToList();
                return View(budget);
            }

            try
            {
                var existing = _db.BudgetLimits.FirstOrDefault(x => x.Id == id && x.UserId == UserId);
                if (existing == null) return NotFound();

                existing.CategoryId = budget.CategoryId;
                existing.LimitAmount = budget.LimitAmount;
                existing.Month = budget.Month;
                existing.Year = budget.Year;

                _db.SaveChanges();
                TempData["Success"] = "Đã cập nhật hạn mức!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra, vui lòng thử lại.");
                ViewBag.Categories = _db.Categories
                    .Where(c => c.Type == TransactionType.Expense).ToList();
                return View(budget);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var b = _db.BudgetLimits.FirstOrDefault(x => x.Id == id && x.UserId == UserId);
                if (b != null)
                {
                    _db.BudgetLimits.Remove(b);
                    _db.SaveChanges();
                    TempData["Success"] = "Đã xóa hạn mức.";
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa, vui lòng thử lại.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}