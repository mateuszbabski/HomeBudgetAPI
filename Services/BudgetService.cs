using AutoMapper;
using HomeBudget.Authorization;
using HomeBudget.Entities;
using HomeBudget.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HomeBudget.Services
{
    public interface IBudgetService
    {
        int Create(CreateBudgetModel dto, int userId, ClaimsPrincipal user);
        void Delete(int id, ClaimsPrincipal user);
        IEnumerable<BudgetModel> GetAll(ClaimsPrincipal user);
        BudgetModel GetById(int id, ClaimsPrincipal user);
        void Update(int id, UpdateBudget dto, ClaimsPrincipal user);
    }

    public class BudgetService : IBudgetService
    {
        private readonly BudgetDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        

        public BudgetService(BudgetDbContext dbContext,
            IMapper mapper,
            IAuthorizationService authorizationService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _authorizationService = authorizationService;
            
        }

        public BudgetModel GetById(int id, ClaimsPrincipal user)
        {
            var userId = int.Parse(user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var budget = _dbContext.Budgets
                .Where(b => b.UserID == userId)
                .Include(t => t.Transactions)
                .FirstOrDefault(m => m.Id == id);

            if(budget is null)
            {
                throw new Exception("Budget not found");
            }

            



            var result = _mapper.Map<BudgetModel>(budget);
            return result;
        }

        public IEnumerable<BudgetModel> GetAll(ClaimsPrincipal user)
        {
            var userId = int.Parse(user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var budget = _dbContext.Budgets
                .Where(b => b.UserID == userId)
                .Include(t => t.Transactions)
                .ToList();


            if (budget is null)
            {
                throw new Exception("Budget not found");
            }




            var budgetDtos = _mapper.Map<List<BudgetModel>>(budget);

            return budgetDtos;
        }

        public int Create(CreateBudgetModel dto, int userId, ClaimsPrincipal user)
        {
           
            var budget = _mapper.Map<Budget>(dto);
            budget.UserID = userId;

            

            _dbContext.Budgets.Add(budget);
            _dbContext.SaveChanges();

            return budget.Id;
        }

        public void Delete(int id, ClaimsPrincipal user)
        {
            var budget = _dbContext.Budgets
                .FirstOrDefault(x => x.Id == id);

            if(budget is null)
            {
                throw new Exception("Budget not found");
            }

            var authorizationResult = _authorizationService.AuthorizeAsync(user,
                budget,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;
            

            if(!authorizationResult.Succeeded)
            {
                throw new Exception("blad autoryzacji");
            }

            _dbContext.Budgets.Remove(budget);
            _dbContext.SaveChanges();
        }

        public void Update(int id, UpdateBudget dto, ClaimsPrincipal user)
        {
            var budget = _dbContext.Budgets
                .FirstOrDefault(x => x.Id == id);


            if (budget is null)
            {
                throw new Exception("Budget not found");
            }

            var authorizationResult = _authorizationService.AuthorizeAsync(user,
                budget,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;


            if (!authorizationResult.Succeeded)
            {
                throw new Exception("blad autoryzacji");
            }


            budget.Name = dto.Name;
            budget.Description = dto.Description;

            _dbContext.SaveChanges();

        }


    }
}
