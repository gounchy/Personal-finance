using CNPM_Nhom12.Models;

namespace CNPM_Nhom12.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance => TotalIncome - TotalExpense;
        public int TransactionCount { get; set; }
        public double IncomeChangePercent { get; set; }
        public double ExpenseChangePercent { get; set; }
        public List<Transaction> RecentTransactions { get; set; } = new();
        public List<BudgetLimit> BudgetLimits { get; set; } = new();
        public List<BudgetLimit> WarningBudgets =>
            BudgetLimits.Where(b => b.Percentage >= 80).ToList();
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
