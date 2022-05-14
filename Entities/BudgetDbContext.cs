using Microsoft.EntityFrameworkCore;

namespace HomeBudget.Entities
{
    public class BudgetDbContext : DbContext
    {
        
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
       

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BudgetDB;Trusted_Connection=true;");
        }
    }

}
