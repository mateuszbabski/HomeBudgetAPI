using AutoMapper;
using HomeBudget.Authorization;
using HomeBudget.Entities;
using HomeBudget.Exceptions;
using HomeBudget.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public BudgetService(BudgetDbContext dbContext,
            IMapper mapper,
            IAuthorizationService authorizationService, 
            IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }

        public BudgetModel GetById(int id)
        {
            var userId = (int)_userContextService.GetUserId;

            var budget = _dbContext.Budgets
                .Where(b => b.UserID == userId)
                .Include(t => t.Transactions)
                .FirstOrDefault(m => m.Id == id);

            if(budget is null)
            {
                throw new NotFoundException("Budget not found");
            }

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User,
                budget,
                new ResourceOperationRequirement(ResourceOperation.Read)).Result;


            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException("Forbidden - Authorization error");
            }


            var result = _mapper.Map<BudgetModel>(budget);
            return result;
        }




        public IEnumerable<BudgetModel> GetAll()
        {
            var userId = (int)_userContextService.GetUserId;

            var budget =  _dbContext.Budgets
                .Where(b => b.UserID == userId)
                .Include(t => t.Transactions)
                .OrderBy(n => n.Name)
                .ToList();


            if (budget is null)
            {
                throw new NotFoundException("Budget not found");
            }


            var budgetDtos = _mapper.Map<List<BudgetModel>>(budget);

            return budgetDtos;
        }



        public int Create(CreateBudgetModel dto)
        {
           
            var budget = _mapper.Map<Budget>(dto);
            budget.UserID = (int)_userContextService.GetUserId;

            
            _dbContext.Budgets.Add(budget);
            _dbContext.SaveChanges();

            return budget.Id;
        }


        public void Delete(int id)
        {
            var budget = _dbContext.Budgets
                .FirstOrDefault(x => x.Id == id);

            if(budget is null)
            {
                throw new NotFoundException("Budget not found");
            }

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User,
                budget,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;
            

            if(!authorizationResult.Succeeded)
            {
                throw new ForbidException("Forbidden - Authorization error");
            }

            _dbContext.Budgets.Remove(budget);
            _dbContext.SaveChanges();
        }

        public void Update(int id, UpdateBudget dto)
        {
            var budget = _dbContext.Budgets
                .FirstOrDefault(x => x.Id == id);


            if (budget is null)
            {
                throw new NotFoundException("Budget not found");
            }

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User,
                budget,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;


            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException("Forbidden - Authorization error");
            }


            budget.Name = dto.Name;
            budget.Description = dto.Description;

            _dbContext.SaveChanges();

        }


    }
}
