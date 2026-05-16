using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM_Nhom12.Models
{
    public class BudgetLimit
    {
        public int Id { get; set; }

        // Gắn ngân sách với user
        [Required]
        public string UserId { get; set; } = "";

        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,0)")]
        [Range(1, double.MaxValue, ErrorMessage = "Hạn mức phải lớn hơn 0")]
        public decimal LimitAmount { get; set; }

        [NotMapped]
        public decimal SpentAmount { get; set; }

        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        [NotMapped]
        public decimal RemainingAmount => LimitAmount - SpentAmount;

        [NotMapped]
        public double Percentage => LimitAmount > 0
            ? (double)(SpentAmount / LimitAmount * 100) : 0;

        [NotMapped]
        public string StatusBadge => Percentage switch
        {
            > 100 => "danger",
            >= 80 => "warning",
            _ => "success"
        };

        [NotMapped]
        public string StatusLabel => Percentage switch
        {
            > 100 => "Vượt mức",
            >= 80 => "Sắp vượt",
            _ => "Bình thường"
        };
    }
}
