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
    public class TransactionsController : Controller
    {
        private readonly AppDbContext _db;
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        private const int PageSize = 8;

        public TransactionsController(AppDbContext db) => _db = db;

        public IActionResult Index(int page = 1, string? monthYear = null,
                                   int? month = null, int? year = null,
                                   int? categoryId = null, string? type = null)
        {
            if (!string.IsNullOrWhiteSpace(monthYear))
            {
                var i = monthYear.IndexOf('-', StringComparison.Ordinal);
                if (i > 0 && int.TryParse(monthYear.AsSpan(0, i), out var y)
                          && int.TryParse(monthYear.AsSpan(i + 1), out var m))
                { year = y; month = m; }
            }

            var query = _db.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == UserId)
                .AsQueryable();

            if (month.HasValue && year.HasValue)
                query = query.Where(t => t.Date.Month == month && t.Date.Year == year);

            if (!string.IsNullOrEmpty(type))
            {
                var txType = type == "income" ? TransactionType.Income : TransactionType.Expense;
                query = query.Where(t => t.Type == txType);
            }

            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId);

            var total = query.Count();
            var paged = query.OrderByDescending(t => t.Date)
                             .Skip((page - 1) * PageSize)
                             .Take(PageSize)
                             .ToList();

            return View(new TransactionViewModel
            {
                Transactions = paged,
                Categories = _db.Categories.ToList(),
                TotalCount = total,
                Page = page,
                PageSize = PageSize,
                FilterMonth = month,
                FilterYear = year,
                FilterCategoryId = categoryId,
                FilterType = type
            });
        }

        public IActionResult Create()
        {
            return View(new TransactionFormViewModel
            {
                Transaction = new Transaction { Date = DateTime.Today },
                Categories = _db.Categories.ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(TransactionFormViewModel vm)
        {
            vm.Transaction.UserId = UserId;
            ModelState.Remove("Transaction.UserId");
            ModelState.Remove("Transaction.Category");
            ModelState.Remove("Categories");

            if (!ModelState.IsValid)
            {
                vm.Categories = _db.Categories.ToList();
                return View(vm);
            }

            try
            {
                _db.Transactions.Add(vm.Transaction);
                _db.SaveChanges();
                TempData["Success"] = "Thêm giao dịch thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra, vui lòng thử lại.");
                vm.Categories = _db.Categories.ToList();
                return View(vm);
            }
        }

        public IActionResult Edit(int id)
        {
            var tx = _db.Transactions.FirstOrDefault(t => t.Id == id && t.UserId == UserId);
            if (tx == null) return NotFound();

            return View(new TransactionFormViewModel
            {
                Transaction = tx,
                Categories = _db.Categories.ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TransactionFormViewModel vm)
        {
            ModelState.Remove("Transaction.UserId");
            ModelState.Remove("Transaction.Category");
            ModelState.Remove("Categories");

            if (!ModelState.IsValid)
            {
                vm.Categories = _db.Categories.ToList();
                return View(vm);
            }

            try
            {
                var existing = _db.Transactions.FirstOrDefault(t => t.Id == id && t.UserId == UserId);
                if (existing == null) return NotFound();

                existing.Description = vm.Transaction.Description;
                existing.Note = vm.Transaction.Note;
                existing.Amount = vm.Transaction.Amount;
                existing.Date = vm.Transaction.Date;
                existing.Type = vm.Transaction.Type;
                existing.CategoryId = vm.Transaction.CategoryId;

                _db.SaveChanges();
                TempData["Success"] = "Cập nhật giao dịch thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra, vui lòng thử lại.");
                vm.Categories = _db.Categories.ToList();
                return View(vm);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var tx = _db.Transactions.FirstOrDefault(t => t.Id == id && t.UserId == UserId);
                if (tx != null)
                {
                    _db.Transactions.Remove(tx);
                    _db.SaveChanges();
                    TempData["Success"] = "Đã xóa giao dịch.";
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