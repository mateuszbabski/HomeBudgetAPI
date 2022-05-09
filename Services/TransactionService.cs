using AutoMapper;
using HomeBudget.Entities;
using HomeBudget.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBudget.Services
{
    public interface ITransactionService
    {
        int Create(CreateTransactionModel dto, int budgetId);
        void Delete(int id, int budgetId);
        IEnumerable<TransactionModel> GetAll(int budgetId);
        
        TransactionModel GetById(int id, int budgetId);
        void Update(UpdateTransactionModel dto, int id, int budgetId);
    }

    public class TransactionService : ITransactionService
    {
        private readonly BudgetDbContext _dbContext;
        private readonly IMapper _mapper;

        public TransactionService(BudgetDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public TransactionModel GetById(int id, int budgetId)
        {
            var budget = GetBudgetById(budgetId);

            var transaction = _dbContext.Transactions.FirstOrDefault(x => x.Id == id);

            if(transaction is null || transaction.BudgetID != budgetId)
            {
                throw new Exception("Transaction not found");
            }

            var result = _mapper.Map<TransactionModel>(transaction);

            return result;
        }

        public IEnumerable<TransactionModel> GetAll(int budgetId)
        {
            var budget = GetBudgetById(budgetId);

            //var transactions = _dbContext.Transactions.ToList();

            var transactionsDto = _mapper.Map<List<TransactionModel>>(budget.Transactions);

            return transactionsDto;
        }

        public int Create(CreateTransactionModel dto, int budgetId)
        {
            var budget = GetBudgetById(budgetId);
            var transaction = _mapper.Map<Transaction>(dto);

            transaction.BudgetID = budgetId;
            

            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return transaction.Id;
        }

        public void Update(UpdateTransactionModel dto, int id, int budgetId)
        {
            var budget = GetBudgetById(budgetId);
            var transaction = _dbContext.Transactions.FirstOrDefault(x => x.Id == id);

            if (transaction is null || transaction.BudgetID != budgetId)
            {
                throw new Exception("Transaction not found");
            }

            transaction.BudgetID = dto.BudgetID;
            transaction.Type = dto.Type;
            transaction.Category = dto.Category;
            transaction.Description = dto.Description;
            transaction.Value = dto.Value; 
            transaction.TransactionDate = dto.TransactionDate;

            _dbContext.SaveChanges();
        }

        public void Delete(int id, int budgetId)
        {
            var budget = GetBudgetById(budgetId);

            var transaction = _dbContext.Transactions.FirstOrDefault(x => x.Id == id);

            if (transaction is null || transaction.BudgetID != budgetId)
            {
                throw new Exception("Transaction not found");
            }

            _dbContext.Transactions.Remove(transaction);
            _dbContext.SaveChanges();
        }

        public Budget GetBudgetById(int budgetId)
        {
            var budget = _dbContext.Budgets.Include(r => r.Transactions).FirstOrDefault(x => x.Id == budgetId);

            if(budget is null)
            {
                throw new Exception("Budget Id not found");
            }

            return budget;
        }
    }
}
