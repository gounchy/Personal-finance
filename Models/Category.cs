namespace CNPM_Nhom12.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Icon { get; set; } = "ti ti-tag";
        public string Color { get; set; } = "#1D9E75";
        public string BgColor { get; set; } = "#E1F5EE";
        public TransactionType Type { get; set; }
        public List<Transaction> Transactions { get; set; } = new();
    }
}
