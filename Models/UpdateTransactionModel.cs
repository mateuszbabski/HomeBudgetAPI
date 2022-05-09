namespace HomeBudget.Models
{
    public class UpdateTransactionModel
    {
        public int BudgetID { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public decimal Value { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
    }
}
