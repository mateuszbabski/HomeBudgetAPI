namespace HomeBudget.Entities
{
    public class Budget
    {
        public int Id { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }    
        public string? Description { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
        public virtual User User { get; set; }

        


        

    }
}


        


        

