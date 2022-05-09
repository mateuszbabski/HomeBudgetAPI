namespace HomeBudget.Models
{
    public class BudgetModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string? Description { get; set; }
        
        
        public virtual List<TransactionModel> Transactions { get; set; }


    }
}
