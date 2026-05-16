using CNPM_Nhom12.Models;

namespace CNPM_Nhom12.ViewModels
{
    public class TransactionViewModel
    {
        public List<Transaction> Transactions { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public int? FilterMonth { get; set; }
        public int? FilterYear { get; set; }
        public int? FilterCategoryId { get; set; }
        public string? FilterType { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
    public class TransactionFormViewModel
    {
        public Transaction Transaction { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
    }
}
