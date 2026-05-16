using CNPM_Nhom12.Models;

namespace CNPM_Nhom12.ViewModels
{
    public class ReportViewModel
    {
        public List<MonthlyData> MonthlyData { get; set; } = new();
        public List<CategorySpend> CategoryBreakdown { get; set; } = new();
        public List<BudgetLimit> BudgetSummary { get; set; } = new();
        public int RangeMonths { get; set; } = 6;
    }

    public class MonthlyData
    {
        public string Label { get; set; } = "";
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
    }

    public class CategorySpend
    {
        public string CategoryName { get; set; } = "";
        public string Color { get; set; } = "";
        public decimal Amount { get; set; }
        public double Percentage { get; set; }
        public decimal? LimitAmount { get; set; }
        public string StatusBadge { get; set; } = "success";
        public string StatusLabel { get; set; } = "Bình thường";
    }
}
