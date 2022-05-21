namespace HomeBudget.Entities
{
    public class Transaction
    {
        public int Id { get; set; } 

        public int BudgetID { get; set; }  

        
        public string Type { get; set; }    
        public string Category { get; set; }    

        public decimal Value { get; set; }
        public DateTime TransactionDate { get; set; }
        
        public string Description { get; set; }

        public virtual Budget Budget { get; set; }

        //public virtual TransactionCategory TransactionCategory { get; set; } 
        //public virtual TransactionType TransactionType { get; set; }
       
    }
}

        









