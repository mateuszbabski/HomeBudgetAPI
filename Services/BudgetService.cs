using AutoMapper;
using HomeBudget.Entities;
using HomeBudget.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBudget.Services
{
    public interface IBudgetService
    {
        int Create(CreateBudgetModel dto);
        void Delete(int id);
        IEnumerable<BudgetModel> GetAll();
        BudgetModel GetById(int id);
        void Update(int id, UpdateBudget dto);
    }

    public class BudgetService : IBudgetService
    {
        private readonly BudgetDbContext _dbContext;
        private readonly IMapper _mapper;

        public BudgetService(BudgetDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;

        }

        public BudgetModel GetById(int id)
        {
            var budget = _dbContext.Budgets.Include(t => t.Transactions).FirstOrDefault(m => m.Id == id);

            if(budget is null)
            {
                throw new Exception("Budget not found");
            }

            var result = _mapper.Map<BudgetModel>(budget);
            return result;
        }

        public IEnumerable<BudgetModel> GetAll()
        {
            var budget = _dbContext.Budgets.Include(t => t.Transactions).ToList();

            var budgetDtos = _mapper.Map<List<BudgetModel>>(budget);

            return budgetDtos;
        }

        public int Create(CreateBudgetModel dto)
        {
            var budget = _mapper.Map<Budget>(dto);

           

            _dbContext.Budgets.Add(budget);
            _dbContext.SaveChanges();

            return budget.Id;
        }

        public void Delete(int id)
        {
            var budget = _dbContext.Budgets.FirstOrDefault(x => x.Id == id);

            if(budget is null)
            {
                throw new Exception("Budget not found");
            }

            _dbContext.Budgets.Remove(budget);
            _dbContext.SaveChanges();
        }

        public void Update(int id, UpdateBudget dto)
        {
            var budget = _dbContext.Budgets.FirstOrDefault(x => x.Id == id);

            if (budget is null)
            {
                throw new Exception("Budget not found");
            }

            budget.Name = dto.Name;
            budget.Description = dto.Description;

            _dbContext.SaveChanges();

        }


    }
}
