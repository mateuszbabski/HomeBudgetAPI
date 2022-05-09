using HomeBudget.Entities;
using Type = HomeBudget.Entities.Type;

namespace HomeBudget
{
    public class BudgetSeeder
    {
        private readonly BudgetDbContext _dbContext;

        public BudgetSeeder(BudgetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                if (!_dbContext.Budgets.Any())
                {
                    var types = GetTypes();
                    _dbContext.Types.AddRange(types);
                    _dbContext.SaveChanges();
                }
                if (!_dbContext.Budgets.Any())
                {
                    var categories = GetCategories();
                    _dbContext.Categories.AddRange(categories);
                    _dbContext.SaveChanges();
                }
                if (!_dbContext.Budgets.Any())
                {
                    var budget = GetBudget();
                    _dbContext.Budgets.AddRange(budget);
                    _dbContext.SaveChanges();
                }
            }
        }

        private IEnumerable<Type> GetTypes()
        {
            var types = new List<Type>()
            {

            new Type()
            {
                Name = "Income"
            },
            new Type()
            {
                Name = "Expense"
            },

            };
            return types;
        }

        private IEnumerable<Category> GetCategories()
        {
            var categories = new List<Category>()
            {

            new Category()
            {
                Name = "Job"
            },
            new Category()
            {
                Name = "Investing"
            },
            new Category()
            {
                Name = "Bills"
            },
            new Category()
            {
                Name = "Mortgage"
            },
            new Category()
            {
                Name = "Groceries"
            },
            new Category()
            {
                Name = "Fuel"
            },
            new Category()
            {
                Name = "Education"
            },
            new Category()
            {
                Name = "Traveling"
            },
            new Category()
            {
                Name = "Stimulants"
            },
            new Category()
            {
                Name = "Hang out"
            },
            new Category()
            {
                Name = "Others"
            },
            };
            return categories;
        }


        public IEnumerable<Budget> GetBudget()
        {
            var budget = new List<Budget>()
            {

            new Budget()
            {
                Name = "Home Budget",
                Description = "Tracking home budget",
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Type = "Income",
                        Category = "Job",
                        Value = 2000,
                        TransactionDate = new DateTime(2022, 5, 6)
                    },
                    new Transaction()
                    {
                        Type = "Expense",
                        Category = "Groceries",
                        Value = 600,
                        TransactionDate = new DateTime(2022, 5, 6)
                    }
                },
            },

            new Budget()
            {
                Name = "Investment Budget",
                Description = "Tracking investing process",
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                    Type = "Expense",
                    Category = "Investing",
                    Value = 500,
                    TransactionDate = new DateTime(2022, 5, 6)
                    },
                },

            }
            };
            return budget;
        }

    }
}
