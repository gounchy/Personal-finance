using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM_Nhom12.Models
{
    public enum TransactionType { Income, Expense }
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        [StringLength(200)]
        public string Description { get; set; } = "";

        [StringLength(500)]
        public string? Note { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,0)")]
        [Range(1, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public TransactionType Type { get; set; }

        [Required]
        public int CategoryId { get; set; }

        // Navigation property
        public Category? Category { get; set; }
    }
}