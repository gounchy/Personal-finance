using System.ComponentModel.DataAnnotations;

namespace CNPM_Nhom12.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(100, ErrorMessage = "Tên không quá 100 ký tự")]
        public string Name { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string Icon { get; set; } = "ti ti-tag";

        [Required]
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Màu không hợp lệ")]
        public string Color { get; set; } = "#1D9E75";

        [Required]
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Màu nền không hợp lệ")]
        public string BgColor { get; set; } = "#E1F5EE";

        public TransactionType Type { get; set; }
        public List<Transaction> Transactions { get; set; } = new();
    }
}
