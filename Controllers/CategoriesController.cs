using CNPM_Nhom12.Data;
using CNPM_Nhom12.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CNPM_Nhom12.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _db;

        public CategoriesController(AppDbContext db) => _db = db;

        public IActionResult Index() => View(_db.Categories.ToList());

        public IActionResult Create() => View(new Category());

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid) return View(category);

            try
            {
                _db.Categories.Add(category);
                _db.SaveChanges();
                TempData["Success"] = "Thêm danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra, vui lòng thử lại.");
                return View(category);
            }
        }

        public IActionResult Edit(int id)
        {
            var cat = _db.Categories.Find(id);
            if (cat == null) return NotFound();
            return View(cat);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)
        {
            if (!ModelState.IsValid) return View(category);

            try
            {
                var existing = _db.Categories.Find(id);
                if (existing == null) return NotFound();

                existing.Name = category.Name;
                existing.Icon = category.Icon;
                existing.Color = category.Color;
                existing.BgColor = category.BgColor;
                existing.Type = category.Type;

                _db.SaveChanges();
                TempData["Success"] = "Cập nhật danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra, vui lòng thử lại.");
                return View(category);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var cat = _db.Categories.Find(id);
            if (cat != null)
            {
                var hasTransactions = _db.Transactions.Any(t => t.CategoryId == id);
                if (hasTransactions)
                {
                    TempData["Error"] = "Không thể xóa vì đã có giao dịch thuộc danh mục này.";
                    return RedirectToAction(nameof(Index));
                }

                try
                {
                    _db.Categories.Remove(cat);
                    _db.SaveChanges();
                    TempData["Success"] = "Đã xóa danh mục.";
                }
                catch (Exception)
                {
                    TempData["Error"] = "Có lỗi xảy ra khi xóa, vui lòng thử lại.";
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}